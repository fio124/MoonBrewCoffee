namespace MoonBrewCoffee.Models.Entidades
{
    public class MenuProducto
    {
        public int IdMenu { get; set; }

        public int IdProducto { get; set; }

        public Menu? Menu { get; set; }

        public Producto? Producto { get; set; }
    }
}