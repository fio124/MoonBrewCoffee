using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class EstadoPedido
    {
        [Key]
        public int IdEstado { get; set; }

        public string Nombre { get; set; }

        public string? ColorHex { get; set; }
    }
}