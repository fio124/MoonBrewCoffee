using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Combo
    {
        [Key]
        public int IdCombo { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public decimal PrecioCombo { get; set; }

        public string? ImagenURL { get; set; }

        public bool Activo { get; set; }
        public ICollection<MenuCombo> MenuCombos { get; set; } = new List<MenuCombo>();
    }
}