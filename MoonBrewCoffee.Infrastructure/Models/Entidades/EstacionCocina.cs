using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class EstacionCocina
    {
        [Key]
        public int IdEstacion { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? ColorHex { get; set; }

        public bool Activo { get; set; }
    }
}
