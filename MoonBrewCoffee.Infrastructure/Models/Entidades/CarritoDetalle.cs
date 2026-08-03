using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class CarritoDetalle
    {
        [Key]
        public int IdCarritoDetalle { get; set; }

        public int IdCarrito { get; set; }

        public int? IdProducto { get; set; }

        public int? IdCombo { get; set; }

        public int Cantidad { get; set; }

        public string? Observaciones { get; set; }

        public Carrito? Carrito { get; set; }

        public Producto? Producto { get; set; }

        public Combo? Combo { get; set; }
    }
}