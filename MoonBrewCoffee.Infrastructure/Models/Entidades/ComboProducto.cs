namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class ComboProducto
    {
        public int IdCombo { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        public Combo? Combo { get; set; }

        public Producto? Producto { get; set; }
    }
}