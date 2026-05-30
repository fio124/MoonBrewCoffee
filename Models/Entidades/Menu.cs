using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Menu
    {
        [Key]
        public int IdMenu { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool Disponible { get; set; }

        public bool Activo { get; set; }
    }
}