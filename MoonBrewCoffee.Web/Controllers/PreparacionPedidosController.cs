using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Web.Controllers
{
    public class PreparacionPedidosController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly ICurrentUserService _currentUserService;

        public PreparacionPedidosController(IPedidoService pedidoService, ICurrentUserService currentUserService)
        {
            _pedidoService = pedidoService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var current = _currentUserService.GetCurrent();
            if (current is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = Url.Action(nameof(Index)) });
            if (!current.EsAdministrador && !current.EsEncargado)
                return Forbid();

            return View(await _pedidoService.GetPreparationBoardAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avanzar(int id, bool completar)
        {
            var current = _currentUserService.GetCurrent();
            if (current is null || (!current.EsAdministrador && !current.EsEncargado))
                return Forbid();

            try
            {
                await _pedidoService.AdvanceProcessAsync(id, completar, current.IdUsuario);
                TempData["SuccessMessage"] = completar
                    ? "Etapa completada; el estado del pedido se actualizó automáticamente."
                    : "Preparación iniciada; el pedido cambió de estado automáticamente.";
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                TempData["ErrorMessage"] = exception.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
