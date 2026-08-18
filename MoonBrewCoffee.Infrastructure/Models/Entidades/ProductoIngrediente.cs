namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class ProductoIngrediente
    {
        public int IdProducto {  get; set; }
        public int IdIngrediente { get; set; }
        public Producto? Producto { get; set; }
        public Ingrediente? Ingrediente { get; set; }
    }
}
