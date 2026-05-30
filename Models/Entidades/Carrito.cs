using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Carrito
    {
        [Key]
        public int IdCarrito { get; set; }

        public int IdUsuario { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime UltimaActualizacion { get; set; }

        public bool Activo { get; set; }

        public Usuario? Usuario { get; set; }
    }
}