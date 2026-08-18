namespace MoonBrewCoffee.Application.DTOs
{
    public class PedidoDTO
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteCorreo { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;
        public string EncargadoNombre { get; set; } = string.Empty;
        public string TipoEntrega { get; set; } = string.Empty;
        public string? DireccionEntrega { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? EstadoColor { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();
        public List<PedidoEstadoCambioDTO> HistorialEstados { get; set; } = new();
    }

    public class PedidoEstadoCambioDTO
    {
        public string Estado { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
        public DateTime FechaCambio { get; set; }
        public string Responsable { get; set; } = string.Empty;
    }

    public class PedidoDetalleDTO
    {
        public string Tipo { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public string? Observaciones { get; set; }
    }

    public class RegistrarPedidoDTO
    {
        public string ClaveOperacion { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public int? IdEncargado { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string? DireccionEntrega { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public decimal? MontoEfectivo { get; set; }
        public List<RegistrarPedidoDetalleDTO> Detalles { get; set; } = new();
    }

    public class RegistrarPedidoDetalleDTO
    {
        public string Tipo { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int Cantidad { get; set; }
        public string? Observaciones { get; set; }
    }

    public class PedidoCreadoDTO
    {
        public int IdPedido { get; set; }
        public decimal Total { get; set; }
        public decimal Vuelto { get; set; }
        public bool YaExistia { get; set; }
    }

    public class PedidoProcesoDTO
    {
        public int IdPedidoProceso { get; set; }
        public int IdPedido { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Estacion { get; set; } = string.Empty;
        public string? EstacionColor { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool PuedeIniciar { get; set; }
        public bool PuedeCompletar { get; set; }
    }
}
