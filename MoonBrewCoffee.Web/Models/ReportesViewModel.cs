using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Web.Models;

public sealed class ReportesViewModel
{
    public DateTime Desde { get; init; }
    public DateTime Hasta { get; init; }
    public int? IdEstado { get; init; }
    public IReadOnlyList<ReporteEstadoViewModel> Estados { get; init; } = [];
    public IReadOnlyList<PedidoDTO> Pedidos { get; init; } = [];
    public IReadOnlyList<ReporteProductoViewModel> Productos { get; init; } = [];

    public int CantidadPedidos => Pedidos.Count;
    public decimal Subtotal => Pedidos.Sum(x => x.Subtotal);
    public decimal Impuesto => Pedidos.Sum(x => x.Impuesto);
    public decimal Envios => Pedidos.Sum(x => x.CostoEnvio);
    public decimal TotalVentas => Pedidos.Sum(x => x.Total);

    public static ReportesViewModel Crear(
        DateTime desde,
        DateTime hasta,
        int? idEstado,
        IEnumerable<EstadoPedidoDTO> estados,
        IEnumerable<PedidoDTO> pedidos)
    {
        var listaPedidos = pedidos.ToList();
        var productos = listaPedidos
            .SelectMany(pedido => pedido.Detalles)
            .Where(detalle => detalle.Tipo.Equals("Producto", StringComparison.OrdinalIgnoreCase))
            .GroupBy(detalle => detalle.Nombre, StringComparer.OrdinalIgnoreCase)
            .Select(grupo => new ReporteProductoViewModel
            {
                Nombre = grupo.Key,
                Cantidad = grupo.Sum(x => x.Cantidad),
                Total = grupo.Sum(x => x.Subtotal)
            })
            .OrderByDescending(x => x.Cantidad)
            .ThenBy(x => x.Nombre)
            .ToList();

        return new ReportesViewModel
        {
            Desde = desde.Date,
            Hasta = hasta.Date,
            IdEstado = idEstado,
            Estados = estados.Select(x => new ReporteEstadoViewModel
            {
                IdEstado = x.IdEstado,
                Nombre = x.Nombre
            }).ToList(),
            Pedidos = listaPedidos,
            Productos = productos
        };
    }
}

public sealed class ReporteEstadoViewModel
{
    public int IdEstado { get; init; }
    public string Nombre { get; init; } = string.Empty;
}

public sealed class ReporteProductoViewModel
{
    public string Nombre { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public decimal Total { get; init; }
}
