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
        private readonly IComboProductoRepository _comboProductoRepository;
        private readonly IProcesoPreparacionRepository _procesoRepository;
        private readonly IEstacionCocinaRepository _estacionRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IProductoRepository productoRepository,
            IComboRepository comboRepository,
            IUsuarioRepository usuarioRepository,
            IComboProductoRepository comboProductoRepository,
            IProcesoPreparacionRepository procesoRepository,
            IEstacionCocinaRepository estacionRepository)
        {
            _pedidoRepository = pedidoRepository;
            _productoRepository = productoRepository;
            _comboRepository = comboRepository;
            _usuarioRepository = usuarioRepository;
            _comboProductoRepository = comboProductoRepository;
            _procesoRepository = procesoRepository;
            _estacionRepository = estacionRepository;
        }

        public async Task<PedidoCreadoDTO> CreateAsync(RegistrarPedidoDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.ClaveOperacion))
                throw new ArgumentException("No se pudo validar la operación. Actualiza la página e inténtalo de nuevo.");

            var existing = await _pedidoRepository.GetByOperationKeyAsync(request.ClaveOperacion);
            if (existing is not null)
                return new PedidoCreadoDTO { IdPedido = existing.IdPedido, Total = existing.Total, YaExistia = true };

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
            var routeRequests = new List<(int ProductId, string Label, int Quantity)>();
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
                    routeRequests.Add((product.IdProducto, product.Nombre, requestedLine.Cantidad));
                }
                else if (type == "combo")
                {
                    var combo = await _comboRepository.GetSummaryByIdAsync(requestedLine.ItemId);
                    if (combo is null || !combo.Activo)
                        throw new ArgumentException("Uno de los combos ya no está disponible.");
                    unitPrice = combo.PrecioCombo;
                    var comboProducts = await _comboProductoRepository.GetByComboAsync(combo.IdCombo);
                    foreach (var comboProduct in comboProducts)
                        routeRequests.Add((comboProduct.IdProducto, combo.Nombre, requestedLine.Cantidad * Math.Max(1, comboProduct.Cantidad)));
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

            var workflow = new List<PedidoProceso>();
            var sequence = 1;
            var activeStations = await _estacionRepository.GetAllAsync(false);
            foreach (var route in routeRequests)
            {
                var configuredSteps = await _procesoRepository.GetByProductoAsync(route.ProductId);
                if (configuredSteps.Count == 0 && activeStations.Count > 0)
                    configuredSteps.Add(new ProcesoPreparacion { IdProducto = route.ProductId, IdEstacion = activeStations[0].IdEstacion, Orden = 1 });

                foreach (var configuredStep in configuredSteps.OrderBy(step => step.Orden))
                    workflow.Add(new PedidoProceso
                    {
                        IdEstacion = configuredStep.IdEstacion,
                        Orden = sequence++,
                        Descripcion = $"{route.Quantity} × {route.Label}",
                        Estado = "Pendiente"
                    });
            }

            if (workflow.Count == 1)
            {
                var finalStation = activeStations.FirstOrDefault(station =>
                    station.IdEstacion != workflow[0].IdEstacion &&
                    station.Nombre.Contains("empaque", StringComparison.OrdinalIgnoreCase))
                    ?? activeStations.FirstOrDefault(station => station.IdEstacion != workflow[0].IdEstacion);
                if (finalStation is not null)
                    workflow.Add(new PedidoProceso
                    {
                        IdEstacion = finalStation.IdEstacion,
                        Orden = sequence++,
                        Descripcion = "Revisión final y entrega",
                        Estado = "Pendiente"
                    });
            }

            if (workflow.Count == 0)
                throw new InvalidOperationException("No hay estaciones activas para preparar el pedido.");

            var order = new Pedido
            {
                ClaveOperacion = request.ClaveOperacion.Trim(),
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
                Detalles = details,
                Procesos = workflow
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

        public async Task<List<PedidoProcesoDTO>> GetPreparationBoardAsync()
        {
            var steps = await _pedidoRepository.GetPreparationBoardAsync();
            var firstByOrder = steps.GroupBy(step => step.IdPedido)
                .ToDictionary(group => group.Key, group => group.OrderBy(step => step.Orden).First().IdPedidoProceso);

            return steps.Select(step => new PedidoProcesoDTO
            {
                IdPedidoProceso = step.IdPedidoProceso,
                IdPedido = step.IdPedido,
                Cliente = $"{step.Pedido?.Cliente?.Nombre} {step.Pedido?.Cliente?.Apellido}".Trim(),
                Estacion = step.Estacion?.Nombre ?? "Sin estación",
                EstacionColor = step.Estacion?.ColorHex,
                Orden = step.Orden,
                Descripcion = step.Descripcion,
                Estado = step.Estado,
                FechaInicio = step.FechaInicio,
                FechaFin = step.FechaFin,
                PuedeIniciar = firstByOrder[step.IdPedido] == step.IdPedidoProceso && step.Estado == "Pendiente",
                PuedeCompletar = step.Estado == "En preparación"
            }).ToList();
        }

        public Task AdvanceProcessAsync(int processId, bool complete, int? userId) =>
            _pedidoRepository.AdvanceProcessAsync(processId, complete, userId);

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
            HistorialEstados = order.HistorialEstados
                .OrderBy(change => change.FechaCambio)
                .Select(change => new PedidoEstadoCambioDTO
                {
                    Estado = change.Estado?.Nombre ?? "Sin estado",
                    ColorHex = change.Estado?.ColorHex,
                    FechaCambio = change.FechaCambio,
                    Responsable = change.Usuario is null
                        ? "Sistema MoonBrew"
                        : $"{change.Usuario.Nombre} {change.Usuario.Apellido}".Trim()
                }).ToList(),
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
