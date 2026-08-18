using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Web.Controllers
{
    public sealed class TareasProgramadasController : Controller
    {
        private readonly MoonBrewContext _context;
        private readonly ScheduledTasksOptions _options;
        private readonly MoonBrewScheduledTasksService _taskRunner;

        public TareasProgramadasController(
            MoonBrewContext context,
            IOptions<ScheduledTasksOptions> options,
            MoonBrewScheduledTasksService taskRunner)
        {
            _context = context;
            _options = options.Value;
            _taskRunner = taskRunner;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var tardeando = await _context.Menus
                .AsNoTracking()
                .FirstOrDefaultAsync(menu => menu.Nombre == _options.TardeandoMenuName, cancellationToken);

            var executions = await _context.ScheduledTaskExecutions
                .AsNoTracking()
                .OrderByDescending(item => item.IdScheduledTaskExecution)
                .Take(20)
                .Select(item => new ScheduledTaskExecutionViewModel
                {
                    TaskName = item.TaskName,
                    StartedAtUtc = item.StartedAtUtc,
                    CompletedAtUtc = item.CompletedAtUtc,
                    Status = item.Status,
                    Details = item.Details
                })
                .ToListAsync(cancellationToken);

            var combos = await _context.Combos
                .AsNoTracking()
                .Where(combo => combo.Activo)
                .OrderBy(combo => combo.Nombre)
                .Select(combo => new ScheduledComboSnapshotViewModel
                {
                    Name = combo.Nombre,
                    Price = combo.PrecioCombo,
                    IsTardeando = combo.MenuCombos.Any(menuCombo =>
                        menuCombo.Menu != null && menuCombo.Menu.Nombre == _options.TardeandoMenuName)
                })
                .ToListAsync(cancellationToken);

            var model = new ScheduledTasksDashboardViewModel
            {
                Enabled = _options.Enabled,
                PriceChangesEnabled = _options.PriceChangesEnabled,
                TardeandoMenuName = _options.TardeandoMenuName,
                TardeandoIntervalSeconds = _options.TardeandoIntervalSeconds,
                GeneralIntervalSeconds = _options.GeneralComboIntervalSeconds,
                DiscountPercent = _options.TardeandoDiscountPercent,
                MinimumPrice = _options.MinimumComboPrice,
                GeneralIncrease = _options.GeneralComboIncrease,
                MaxExecutionsPerRun = _options.MaxPriceExecutionsPerRun,
                TardeandoExecutionsThisRun = _taskRunner.TardeandoPriceExecutions,
                GeneralExecutionsThisRun = _taskRunner.GeneralPriceExecutions,
                LastTardeandoChanges = _taskRunner.LastTardeandoChanges
                    .Select(change => new ScheduledPriceChangeViewModel(change.Name, change.Before, change.After))
                    .ToList(),
                LastGeneralChanges = _taskRunner.LastGeneralChanges
                    .Select(change => new ScheduledPriceChangeViewModel(change.Name, change.Before, change.After))
                    .ToList(),
                TardeandoAvailable = tardeando?.Disponible,
                TardeandoSchedule = tardeando is null
                    ? null
                    : $"{tardeando.HoraInicio:hh\\:mm} - {tardeando.HoraFin:hh\\:mm}",
                Executions = executions,
                Combos = combos
            };

            return View(model);
        }
    }
}
