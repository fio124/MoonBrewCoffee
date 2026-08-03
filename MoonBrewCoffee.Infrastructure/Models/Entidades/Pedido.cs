using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }

        public int? IdEncargado { get; set; }

        public int IdEstado { get; set; }

        public DateTime FechaPedido { get; set; }

        public string TipoEntrega { get; set; } = string.Empty;

        public string? DireccionEntrega { get; set; }

        public decimal CostoEnvio { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public string? Observaciones { get; set; }

        public bool Activo { get; set; }

        public Usuario? Cliente { get; set; }

        public Usuario? Encargado { get; set; }

        public EstadoPedido? EstadoPedido { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

        public Pago? Pago { get; set; }
    }
}
