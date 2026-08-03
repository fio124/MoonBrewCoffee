using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class Menu
    {
        [Key]
        public int IdMenu { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public string? ImagenURL { get; set; }

        public bool Disponible { get; set; }

        public bool Activo { get; set; }

        public ICollection<MenuProducto> MenuProductos { get; set; }
            = new List<MenuProducto>();

        public ICollection<MenuCombo> MenuCombos { get; set; }
            = new List<MenuCombo>();
    }
}