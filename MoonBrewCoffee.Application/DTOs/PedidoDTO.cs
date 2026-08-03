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
    }
}
