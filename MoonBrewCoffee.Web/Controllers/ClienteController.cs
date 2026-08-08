using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IComboService _comboService;
        private readonly IMenuService _menuService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICartService _cartService;
        private readonly IWeatherService _weatherService;

        public ClienteController(
            IProductoService productoService,
            IComboService comboService,
            IMenuService menuService,
            ICurrentUserService currentUserService,
            ICartService cartService,
            IWeatherService weatherService)
        {
            _productoService = productoService;
            _comboService = comboService;
            _menuService = menuService;
            _currentUserService = currentUserService;
            _cartService = cartService;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            var weatherTask = _weatherService.GetSanJoseAsync(HttpContext.RequestAborted);
            var menusDisponibles = await _menuService.GetMenusDisponiblesActualesAsync();

            return View(new ClienteHomeViewModel
            {
                ProductosDisponibles = await _productoService.CountActiveAsync(),
                CombosDisponibles = await _comboService.CountActiveAsync(),
                MenusDisponibles = menusDisponibles.Count,
                Weather = await weatherTask
            });
        }

        public async Task<IActionResult> Carrito() => View(await _cartService.GetAsync());

        public IActionResult MisPedidos() => RedirectToAction("Index", "Pedidos");

        public IActionResult Perfil()
        {
            var currentUser = _currentUserService.GetCurrent();
            if (currentUser is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = "/Cliente/Perfil" });

            return currentUser.EsAdministrador
                ? RedirectToAction("Index", "Home")
                : View(currentUser);
        }
    }
}
