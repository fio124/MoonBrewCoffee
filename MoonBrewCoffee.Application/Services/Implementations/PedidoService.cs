using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class PedidoService : IPedidoService
    {
        private const decimal TaxRate = 0.13m;
        private const decimal ShippingCost = 2500m;

        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IComboRepository _comboRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IProductoRepository productoRepository,
            IComboRepository comboRepository,
            IUsuarioRepository usuarioRepository)
        {
            _pedidoRepository = pedidoRepository;
            _productoRepository = productoRepository;
            _comboRepository = comboRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PedidoCreadoDTO> CreateAsync(RegistrarPedidoDTO request)
        {
            var client = await _usuarioRepository.GetByIdAsync(request.IdCliente);
            if (client is null || !client.Activo)
                throw new ArgumentException("El cliente seleccionado no está disponible.");
            if (request.Detalles.Count == 0)
                throw new ArgumentException("Agrega al menos un artículo al pedido.");

            var deliveryType = request.TipoEntrega.Trim().ToLowerInvariant();
            if (deliveryType is not "domicilio" and not "tienda")
                throw new ArgumentException("Selecciona un método de entrega válido.");
            if (deliveryType == "domicilio" && string.IsNullOrWhiteSpace(request.DireccionEntrega))
                throw new ArgumentException("La dirección es obligatoria para la entrega a domicilio.");

            var paymentMethod = request.MetodoPago.Trim().ToLowerInvariant();
            if (paymentMethod is not "credito" and not "debito" and not "efectivo")
                throw new ArgumentException("Selecciona un método de pago válido.");

            var details = new List<DetallePedido>();
            foreach (var requestedLine in request.Detalles)
            {
                if (requestedLine.Cantidad is < 1 or > 99)
                    throw new ArgumentException("Las cantidades deben estar entre 1 y 99.");

                var type = requestedLine.Tipo.Trim().ToLowerInvariant();
                decimal unitPrice;
                if (type == "producto")
                {
                    var product = await _productoRepository.GetSummaryByIdAsync(requestedLine.ItemId);
                    if (product is null || !product.Activo)
                        throw new ArgumentException("Uno de los productos ya no está disponible.");
                    unitPrice = product.Precio;
                }
                else if (type == "combo")
                {
                    var combo = await _comboRepository.GetSummaryByIdAsync(requestedLine.ItemId);
                    if (combo is null || !combo.Activo)
                        throw new ArgumentException("Uno de los combos ya no está disponible.");
                    unitPrice = combo.PrecioCombo;
                }
                else
                {
                    throw new ArgumentException("Uno de los artículos del pedido no es válido.");
                }

                var subtotal = unitPrice * requestedLine.Cantidad;
                details.Add(new DetallePedido
                {
                    IdProducto = type == "producto" ? requestedLine.ItemId : null,
                    IdCombo = type == "combo" ? requestedLine.ItemId : null,
                    Cantidad = requestedLine.Cantidad,
                    PrecioUnitario = unitPrice,
                    Subtotal = subtotal,
                    Impuesto = Math.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero),
                    Observaciones = string.IsNullOrWhiteSpace(requestedLine.Observaciones)
                        ? null
                        : requestedLine.Observaciones.Trim()[..Math.Min(250, requestedLine.Observaciones.Trim().Length)]
                });
            }

            var subtotalTotal = details.Sum(line => line.Subtotal);
            var taxTotal = details.Sum(line => line.Impuesto);
            var shipping = deliveryType == "domicilio" ? ShippingCost : 0m;
            var total = subtotalTotal + taxTotal + shipping;
            var cashReceived = request.MontoEfectivo ?? 0m;
            if (paymentMethod == "efectivo" && cashReceived < total)
                throw new ArgumentException("El monto recibido debe ser igual o mayor al total del pedido.");

            var status = await _pedidoRepository.GetStatusByNameAsync("Aceptada")
                ?? (await _pedidoRepository.GetStatusesAsync()).FirstOrDefault()
                ?? throw new InvalidOperationException("No existen estados de pedido configurados.");

            var order = new Pedido
            {
                IdCliente = request.IdCliente,
                IdEncargado = request.IdEncargado,
                IdEstado = status.IdEstado,
                FechaPedido = DateTime.Now,
                TipoEntrega = deliveryType == "domicilio" ? "Entrega a domicilio" : "Recogida en tienda",
                DireccionEntrega = deliveryType == "domicilio" ? request.DireccionEntrega?.Trim() : null,
                CostoEnvio = shipping,
                Subtotal = subtotalTotal,
                Impuesto = taxTotal,
                Total = total,
                Activo = true,
                Detalles = details
            };
            var payment = new Pago
            {
                MetodoPago = paymentMethod switch
                {
                    "credito" => "Tarjeta de crédito",
                    "debito" => "Tarjeta de débito",
                    _ => "Efectivo"
                },
                FechaPago = DateTime.Now,
                TotalPagado = total,
                EstadoPago = "Aprobado"
            };

            var orderId = await _pedidoRepository.AddAsync(order, payment);
            return new PedidoCreadoDTO
            {
                IdPedido = orderId,
                Total = total,
                Vuelto = paymentMethod == "efectivo" ? cashReceived - total : 0m
            };
        }

        public async Task<PedidoDTO?> GetByIdAsync(int id)
        {
            var order = await _pedidoRepository.GetByIdAsync(id);
            return order is null ? null : Map(order);
        }

        public async Task<List<PedidoDTO>> GetByClientAsync(int clientId) =>
            (await _pedidoRepository.GetByClientAsync(clientId)).Select(Map).ToList();

        public async Task<List<PedidoDTO>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? statusId = null) =>
            (await _pedidoRepository.GetAllAsync(from, to, statusId)).Select(Map).ToList();

        public async Task<List<EstadoPedidoDTO>> GetStatusesAsync() =>
            (await _pedidoRepository.GetStatusesAsync()).Select(status => new EstadoPedidoDTO
            {
                IdEstado = status.IdEstado,
                Nombre = status.Nombre,
                ColorHex = status.ColorHex
            }).ToList();

        private static PedidoDTO Map(Pedido order) => new()
        {
            IdPedido = order.IdPedido,
            FechaPedido = order.FechaPedido,
            IdCliente = order.IdCliente,
            ClienteNombre = $"{order.Cliente?.Nombre} {order.Cliente?.Apellido}".Trim(),
            ClienteCorreo = order.Cliente?.Correo ?? string.Empty,
            ClienteTelefono = order.Cliente?.Telefono ?? string.Empty,
            EncargadoNombre = order.Encargado is null ? "Pedido en línea" : $"{order.Encargado.Nombre} {order.Encargado.Apellido}".Trim(),
            TipoEntrega = order.TipoEntrega,
            DireccionEntrega = order.DireccionEntrega,
            MetodoPago = order.Pago?.MetodoPago ?? "No disponible",
            Estado = order.EstadoPedido?.Nombre ?? "Sin estado",
            EstadoColor = order.EstadoPedido?.ColorHex,
            CostoEnvio = order.CostoEnvio,
            Subtotal = order.Subtotal,
            Impuesto = order.Impuesto,
            Total = order.Total,
            Detalles = order.Detalles.Select(line => new PedidoDetalleDTO
            {
                Tipo = line.IdProducto.HasValue ? "Producto" : "Combo",
                ItemId = line.IdProducto ?? line.IdCombo ?? 0,
                Nombre = line.Producto?.Nombre ?? line.Combo?.Nombre ?? "Artículo",
                PrecioUnitario = line.PrecioUnitario,
                Cantidad = line.Cantidad,
                Subtotal = line.Subtotal,
                Impuesto = line.Impuesto,
                Observaciones = line.Observaciones
            }).ToList()
        };
    }
}
