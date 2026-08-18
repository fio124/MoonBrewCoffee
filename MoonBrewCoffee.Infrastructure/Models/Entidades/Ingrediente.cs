namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Ingrediente
    {
        public int IdIngrediente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
