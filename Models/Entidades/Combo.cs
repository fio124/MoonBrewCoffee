using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Combo
    {
        [Key]
        public int IdCombo { get; set; }

        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public decimal PrecioCombo { get; set; }

        public string? ImagenURL { get; set; }

        public bool Activo { get; set; }
    }
}