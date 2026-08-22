using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Data;
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
        private readonly MoonBrewContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            IProductoService productoService,
            IComboService comboService,
            IMenuService menuService,
            IUsuarioService usuarioService,
            ICurrentUserService currentUserService,
            MoonBrewContext context)
        {
            _logger = logger;
            _productoService = productoService;
            _comboService = comboService;
            _menuService = menuService;
            _usuarioService = usuarioService;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = _currentUserService.GetCurrent();
            if (currentUser is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = "/Home" });

            if (!currentUser.EsAdministrador)
                return RedirectToAction("Index", "Cliente");

            var inicioDia = DateTime.Today;
            var finDia = inicioDia.AddDays(1);

            var pedidosHoy = _context.Pedidos
                .AsNoTracking()
                .Where(p => p.Activo &&
                            p.FechaPedido >= inicioDia &&
                            p.FechaPedido < finDia);

            var resumenPedidos = await pedidosHoy
                .GroupBy(_ => 1)
                .Select(grupo => new
                {
                    Cantidad = grupo.Count(),
                    Total = grupo.Sum(p => p.Total)
                })
                .FirstOrDefaultAsync();

            var productosMasVendidos = await _context.DetallePedidos
                .AsNoTracking()
                .Where(d => d.IdProducto.HasValue &&
                            d.Producto != null &&
                            d.Pedido != null &&
                            d.Pedido.Activo)
                .GroupBy(d => new { d.IdProducto, d.Producto!.Nombre })
                .Select(grupo => new ProductoVendidoViewModel
                {
                    Nombre = grupo.Key.Nombre,
                    Cantidad = grupo.Sum(d => d.Cantidad),
                    TotalVendido = grupo.Sum(d => d.Subtotal)
                })
                .OrderByDescending(x => x.Cantidad)
                .ThenBy(x => x.Nombre)
                .Take(3)
                .ToListAsync();

            var pedidosPorEstadoDatos = await pedidosHoy
                .GroupBy(p => p.EstadoPedido != null
                    ? p.EstadoPedido.Nombre
                    : "Sin estado")
                .Select(grupo => new
                {
                    Estado = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ThenBy(x => x.Estado)
                .ToListAsync();

            var colores = new[] { "#8b5e3c", "#c1844d", "#4f7b62", "#805ad5", "#b7791f", "#9b4a3f" };
            var pedidosPorEstado = pedidosPorEstadoDatos
                .Select((item, indice) => new PedidosPorEstadoViewModel
                {
                    Estado = item.Estado,
                    Cantidad = item.Cantidad,
                    Color = colores[indice % colores.Length]
                })
                .ToList();

            return View(new HomeDashboardViewModel
            {
                ProductosActivos = await _productoService.CountActiveAsync(),
                CombosActivos = await _comboService.CountActiveAsync(),
                MenusActivos = await _menuService.CountActiveAsync(),
                UsuariosActivos = await _usuarioService.CountActiveAsync(),
                PedidosHoy = resumenPedidos?.Cantidad ?? 0,
                VentasHoy = resumenPedidos?.Total ?? 0,
                ProductosMasVendidos = productosMasVendidos,
                PedidosPorEstado = pedidosPorEstado
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
