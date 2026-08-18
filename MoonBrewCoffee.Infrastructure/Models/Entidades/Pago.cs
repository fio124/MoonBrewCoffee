using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        public int IdPedido { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public DateTime FechaPago { get; set; }

        public decimal TotalPagado { get; set; }

        public string EstadoPago { get; set; } = string.Empty;

        public Pedido? Pedido { get; set; }
    }
}
