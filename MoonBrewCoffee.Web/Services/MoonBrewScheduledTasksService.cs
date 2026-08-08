using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Web.Services
{
    public sealed class MoonBrewScheduledTasksService : BackgroundService
    {
        private const string TardeandoTaskName = "TardeandoDiscount";
        private const string GeneralComboTaskName = "GeneralComboIncrease";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ScheduledTasksOptions _options;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<MoonBrewScheduledTasksService> _logger;
        private int _tardeandoPriceExecutions;
        private int _generalPriceExecutions;
        private ScheduledPriceChange[] _lastTardeandoChanges = [];
        private ScheduledPriceChange[] _lastGeneralChanges = [];

        public int TardeandoPriceExecutions => Volatile.Read(ref _tardeandoPriceExecutions);
        public int GeneralPriceExecutions => Volatile.Read(ref _generalPriceExecutions);
        public IReadOnlyList<ScheduledPriceChange> LastTardeandoChanges => Volatile.Read(ref _lastTardeandoChanges);
        public IReadOnlyList<ScheduledPriceChange> LastGeneralChanges => Volatile.Read(ref _lastGeneralChanges);

        public MoonBrewScheduledTasksService(
            IServiceScopeFactory scopeFactory,
            IOptions<ScheduledTasksOptions> options,
            TimeProvider timeProvider,
            ILogger<MoonBrewScheduledTasksService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Las tareas programadas de MoonBrew están deshabilitadas.");
                return Task.CompletedTask;
            }

            var tardeandoInterval = SafeInterval(_options.TardeandoIntervalSeconds);
            var generalInterval = SafeInterval(_options.GeneralComboIntervalSeconds);

            _logger.LogInformation(
                "Tareas programadas iniciadas. Tardeando cada {TardeandoInterval}; combos generales cada {GeneralInterval}; cambios de precio: {PriceChangesEnabled}.",
                tardeandoInterval,
                generalInterval,
                _options.PriceChangesEnabled);

            return RunLoopAsync("Tardeando", tardeandoInterval, RunTardeandoAsync, stoppingToken);
        }

        public Task RunGeneralComboIncreaseOnceAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return RunSafelyAsync("Combos generales", interval, RunGeneralComboIncreaseAsync, cancellationToken);
        }

        private async Task RunLoopAsync(
            string displayName,
            TimeSpan interval,
            Func<TimeSpan, CancellationToken, Task> operation,
            CancellationToken stoppingToken)
        {
            if (_options.RunImmediately)
                await RunSafelyAsync(displayName, interval, operation, stoppingToken);

            using var timer = new PeriodicTimer(interval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                    await RunSafelyAsync(displayName, interval, operation, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Tarea programada {TaskName} detenida correctamente.", displayName);
            }
        }

        private async Task RunSafelyAsync(
            string displayName,
            TimeSpan interval,
            Func<TimeSpan, CancellationToken, Task> operation,
            CancellationToken stoppingToken)
        {
            try
            {
                await operation(interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "La tarea programada {TaskName} falló; el sitio continuará disponible.", displayName);
            }
        }

        private async Task RunTardeandoAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<MoonBrewContext>();
            var localNow = _timeProvider.GetLocalNow().DateTime;
            var utcNow = _timeProvider.GetUtcNow();

            var menu = await context.Menus
                .Include(item => item.MenuCombos)
                .ThenInclude(item => item.Combo)
                .FirstOrDefaultAsync(
                    item => item.Nombre == _options.TardeandoMenuName,
                    cancellationToken);

            if (menu is null)
            {
                _logger.LogWarning("No existe un menú llamado {MenuName}; se omitió la tarea Tardeando.", _options.TardeandoMenuName);
                return;
            }

            var shouldBeAvailable = ScheduledTaskRules.IsMenuAvailable(menu, localNow);
            var availabilityChanged = menu.Disponible != shouldBeAvailable;
            menu.Disponible = shouldBeAvailable;

            if (availabilityChanged)
                await context.SaveChangesAsync(cancellationToken);

            if (!CanChangePrices(ref _tardeandoPriceExecutions) || !shouldBeAvailable)
                return;

            var executionKey = ScheduledTaskRules.BuildExecutionKey(TardeandoTaskName, utcNow, interval);
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var execution = NewExecution(TardeandoTaskName, executionKey, utcNow.UtcDateTime);
            context.ScheduledTaskExecutions.Add(execution);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogDebug("La ejecución {ExecutionKey} ya fue procesada por otra instancia.", executionKey);
                return;
            }

            var changes = new List<ScheduledPriceChange>();
            foreach (var combo in menu.MenuCombos.Select(item => item.Combo).OfType<Combo>().DistinctBy(item => item.IdCombo))
            {
                var previousPrice = combo.PrecioCombo;
                var newPrice = ScheduledTaskRules.ApplyDiscount(
                    previousPrice,
                    _options.TardeandoDiscountPercent,
                    _options.MinimumComboPrice);

                if (newPrice == previousPrice)
                    continue;

                combo.PrecioCombo = newPrice;
                changes.Add(new ScheduledPriceChange(combo.Nombre, previousPrice, newPrice));
            }

            execution.Status = "Completed";
            execution.CompletedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
            execution.Details = $"Disponibilidad: {menu.Disponible}; combos con descuento: {changes.Count}.";
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            Volatile.Write(ref _lastTardeandoChanges, changes.ToArray());
            Interlocked.Increment(ref _tardeandoPriceExecutions);

            _logger.LogInformation(
                "Tardeando actualizado. Disponible: {Available}; combos modificados: {ChangedCombos}.",
                menu.Disponible,
                changes.Count);
        }

        private async Task RunGeneralComboIncreaseAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            if (!CanChangePrices(ref _generalPriceExecutions))
                return;

            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<MoonBrewContext>();
            var utcNow = _timeProvider.GetUtcNow();
            var executionKey = ScheduledTaskRules.BuildExecutionKey(GeneralComboTaskName, utcNow, interval);

            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var execution = NewExecution(GeneralComboTaskName, executionKey, utcNow.UtcDateTime);
            context.ScheduledTaskExecutions.Add(execution);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogDebug("La ejecución {ExecutionKey} ya fue procesada por otra instancia.", executionKey);
                return;
            }

            var combos = await context.Combos
                .Where(combo => combo.Activo && !combo.MenuCombos.Any(menuCombo => menuCombo.Menu != null && menuCombo.Menu.Nombre == _options.TardeandoMenuName))
                .ToListAsync(cancellationToken);

            var changes = new List<ScheduledPriceChange>();
            foreach (var combo in combos)
            {
                var previousPrice = combo.PrecioCombo;
                var newPrice = ScheduledTaskRules.ApplyIncrease(previousPrice, _options.GeneralComboIncrease);
                combo.PrecioCombo = newPrice;
                if (newPrice != previousPrice)
                    changes.Add(new ScheduledPriceChange(combo.Nombre, previousPrice, newPrice));
            }

            execution.Status = "Completed";
            execution.CompletedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
            execution.Details = $"Combos incrementados: {combos.Count}; monto: {_options.GeneralComboIncrease:N2}.";
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            Volatile.Write(ref _lastGeneralChanges, changes.ToArray());
            Interlocked.Increment(ref _generalPriceExecutions);

            _logger.LogInformation("Se incrementó el precio de {ComboCount} combos generales.", combos.Count);
        }

        private bool CanChangePrices(ref int executionCounter)
        {
            return _options.PriceChangesEnabled &&
                _options.MaxPriceExecutionsPerRun > 0 &&
                Volatile.Read(ref executionCounter) < _options.MaxPriceExecutionsPerRun;
        }

        private static TimeSpan SafeInterval(int seconds)
        {
            return TimeSpan.FromSeconds(Math.Clamp(seconds, 5, 86_400));
        }

        private static ScheduledTaskExecution NewExecution(string taskName, string executionKey, DateTime startedAtUtc)
        {
            return new ScheduledTaskExecution
            {
                TaskName = taskName,
                ExecutionKey = executionKey,
                StartedAtUtc = startedAtUtc,
                Status = "Running"
            };
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                sqlException.Number is 2601 or 2627;
        }
    }

    public sealed record ScheduledPriceChange(string Name, decimal Before, decimal After);
}
