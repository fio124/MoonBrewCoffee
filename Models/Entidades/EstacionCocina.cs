using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class EstacionCocina
    {
        [Key]
        public int IdEstacion { get; set; }

        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public string? ColorHex { get; set; }

        public bool Activo { get; set; }
    }
}