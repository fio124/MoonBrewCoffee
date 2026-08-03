using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _currentUserService;

        public PedidosController(
            IPedidoService pedidoService,
            IUsuarioService usuarioService,
            ICartService cartService,
            ICurrentUserService currentUserService)
        {
            _pedidoService = pedidoService;
            _usuarioService = usuarioService;
            _cartService = cartService;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index(DateTime? from = null, DateTime? to = null, int? statusId = null)
        {
            var current = _currentUserService.GetCurrent();
            if (current is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = Url.Action(nameof(Index), "Pedidos") });

            var management = current.EsAdministrador || current.EsEncargado;
            var orders = management
                ? await _pedidoService.GetAllAsync(from, to, statusId)
                : await _pedidoService.GetByClientAsync(current.IdUsuario ?? 0);

            return View(new OrderHistoryViewModel
            {
                Orders = orders,
                Statuses = management ? await _pedidoService.GetStatusesAsync() : new(),
                IsManagementView = management,
                From = from,
                To = to,
                StatusId = statusId
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var current = _currentUserService.GetCurrent();
            if (current is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = Url.Action(nameof(Details), "Pedidos", new { id }) });

            var order = await _pedidoService.GetByIdAsync(id);
            if (order is null)
                return NotFound();
            if (!current.EsAdministrador && !current.EsEncargado && order.IdCliente != current.IdUsuario)
                return Forbid();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var current = _currentUserService.GetCurrent();
            if (current is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = Url.Action(nameof(Create), "Pedidos") });

            var model = await BuildCheckoutAsync(new CheckoutViewModel(), current);
            if (model.Cart.IsEmpty)
            {
                TempData["ErrorMessage"] = "Agrega al menos un artículo antes de continuar.";
                return RedirectToAction("Carrito", "Cliente");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CheckoutViewModel model)
        {
            var current = _currentUserService.GetCurrent();
            if (current is null)
                return RedirectToAction("IniciarSesion", "Cuenta", new { returnUrl = Url.Action(nameof(Create), "Pedidos") });

            model = await BuildCheckoutAsync(model, current);
            ModelState.Clear();
            TryValidateModel(model);
            if (model.Cart.IsEmpty)
                ModelState.AddModelError(string.Empty, "El carrito está vacío.");
            if (!ModelState.IsValid)
                return View(model);

            var clientId = model.CanSelectClient ? model.ClientId!.Value : current.IdUsuario!.Value;
            var request = new RegistrarPedidoDTO
            {
                IdCliente = clientId,
                IdEncargado = model.CanSelectClient ? current.IdUsuario : null,
                TipoEntrega = model.DeliveryType,
                DireccionEntrega = model.DeliveryAddress,
                MetodoPago = model.PaymentMethod,
                MontoEfectivo = model.CashReceived,
                Detalles = model.Cart.Lines.Select(line => new RegistrarPedidoDetalleDTO
                {
                    Tipo = line.ItemType,
                    ItemId = line.ItemId,
                    Cantidad = line.Quantity,
                    Observaciones = line.Notes
                }).ToList()
            };

            try
            {
                var result = await _pedidoService.CreateAsync(request);
                _cartService.Clear();
                TempData["SuccessMessage"] = result.Vuelto > 0
                    ? $"Pedido #{result.IdPedido} registrado. Vuelto: ₡{result.Vuelto:N0}."
                    : $"Pedido #{result.IdPedido} registrado correctamente.";
                return RedirectToAction(nameof(Details), new { id = result.IdPedido });
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View(model);
            }
        }

        private async Task<CheckoutViewModel> BuildCheckoutAsync(CheckoutViewModel model, CurrentUserViewModel current)
        {
            model.Cart = await _cartService.GetAsync();
            model.CurrentUser = current;
            model.CanSelectClient = current.EsAdministrador || current.EsEncargado;
            if (model.CanSelectClient)
            {
                model.Clients = (await _usuarioService.GetAllAsync())
                    .Where(user => user.Activo && user.NombreRol.Contains("cliente", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(user => user.Nombre)
                    .Select(user => new SelectListItem($"{user.Nombre} {user.Apellido} — {user.Correo}", user.IdUsuario.ToString()))
                    .ToList();
            }
            else
            {
                model.ClientId = current.IdUsuario;
            }
            return model;
        }
    }
}
