using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class DetallePedido
    {
        [Key]
        public int IdDetallePedido { get; set; }

        public int IdPedido { get; set; }

        public int? IdProducto { get; set; }

        public int? IdCombo { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public string? Observaciones { get; set; }

        public Pedido? Pedido { get; set; }

        public Producto? Producto { get; set; }

        public Combo? Combo { get; set; }
    }
}
