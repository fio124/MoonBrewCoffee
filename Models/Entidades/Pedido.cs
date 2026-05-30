using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }

        public int IdEstado { get; set; }

        public DateTime FechaPedido { get; set; }

        public string TipoEntrega { get; set; }

        public string? DireccionEntrega { get; set; }

        public decimal CostoEnvio { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public string? Observaciones { get; set; }

        public bool Activo { get; set; }

        public Usuario? Cliente { get; set; }

        public EstadoPedido? EstadoPedido { get; set; }
    }
}