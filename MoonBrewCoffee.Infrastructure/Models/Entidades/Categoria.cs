using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
