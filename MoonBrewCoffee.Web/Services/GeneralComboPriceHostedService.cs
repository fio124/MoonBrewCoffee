using Microsoft.Extensions.Options;

namespace MoonBrewCoffee.Web.Services
{
    public sealed class GeneralComboPriceHostedService : IHostedService, IDisposable
    {
        private readonly MoonBrewScheduledTasksService _taskRunner;
        private readonly ScheduledTasksOptions _options;
        private readonly ILogger<GeneralComboPriceHostedService> _logger;
        private CancellationTokenSource? _stoppingSource;
        private Task? _executingTask;

        public GeneralComboPriceHostedService(
            MoonBrewScheduledTasksService taskRunner,
            IOptions<ScheduledTasksOptions> options,
            ILogger<GeneralComboPriceHostedService> logger)
        {
            _taskRunner = taskRunner;
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
                return Task.CompletedTask;

            _stoppingSource = new CancellationTokenSource();
            _executingTask = ExecuteAsync(_stoppingSource.Token);
            _logger.LogInformation("La tarea IHostedService de combos generales fue iniciada.");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask is null || _stoppingSource is null)
                return;

            _stoppingSource.Cancel();
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            _logger.LogInformation("La tarea IHostedService de combos generales fue detenida correctamente.");
        }

        public void Dispose()
        {
            _stoppingSource?.Cancel();
            _stoppingSource?.Dispose();
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var interval = TimeSpan.FromSeconds(Math.Clamp(_options.GeneralComboIntervalSeconds, 5, 86_400));

            if (_options.RunImmediately)
                await _taskRunner.RunGeneralComboIncreaseOnceAsync(interval, cancellationToken);

            using var timer = new PeriodicTimer(interval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                    await _taskRunner.RunGeneralComboIncreaseOnceAsync(interval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Detención normal solicitada por ASP.NET Core.
            }
        }
    }
}
