using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;
using System.Diagnostics;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoService _productoService;
        private readonly IComboService _comboService;
        private readonly IMenuService _menuService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICurrentUserService _currentUserService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductoService productoService,
            IComboService comboService,
            IMenuService menuService,
            IUsuarioService usuarioService,
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _productoService = productoService;
            _comboService = comboService;
            _menuService = menuService;
            _usuarioService = usuarioService;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = _currentUserService.GetCurrent();
            if (currentUser is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = "/Home" });

            if (!currentUser.EsAdministrador)
                return RedirectToAction("Index", "Cliente");

            return View(new HomeDashboardViewModel
            {
                ProductosActivos = await _productoService.CountActiveAsync(),
                CombosActivos = await _comboService.CountActiveAsync(),
                MenusActivos = await _menuService.CountActiveAsync(),
                UsuariosActivos = await _usuarioService.CountActiveAsync()
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
