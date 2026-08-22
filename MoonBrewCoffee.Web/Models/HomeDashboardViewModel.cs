namespace MoonBrewCoffee.Web.Models
{
    public class HomeDashboardViewModel
    {
        public int ProductosActivos { get; init; }
        public int CombosActivos { get; init; }
        public int MenusActivos { get; init; }
        public int UsuariosActivos { get; init; }
        public int PedidosHoy { get; init; }
        public decimal VentasHoy { get; init; }
        public IReadOnlyList<ProductoVendidoViewModel> ProductosMasVendidos { get; init; } = [];
        public IReadOnlyList<PedidosPorEstadoViewModel> PedidosPorEstado { get; init; } = [];

        public int MayorCantidadProducto =>
            ProductosMasVendidos.Count == 0 ? 0 : ProductosMasVendidos.Max(x => x.Cantidad);

        public int MayorCantidadEstado =>
            PedidosPorEstado.Count == 0 ? 0 : PedidosPorEstado.Max(x => x.Cantidad);
    }

    public sealed class ProductoVendidoViewModel
    {
        public string Nombre { get; init; } = string.Empty;
        public int Cantidad { get; init; }
        public decimal TotalVendido { get; init; }
    }

    public sealed class PedidosPorEstadoViewModel
    {
        public string Estado { get; init; } = string.Empty;
        public int Cantidad { get; init; }
        public string Color { get; init; } = "#8b5e3c";
    }
}
