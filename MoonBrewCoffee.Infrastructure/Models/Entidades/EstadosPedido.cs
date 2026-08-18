using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class EstadoPedido
    {
        [Key]
        public int IdEstado { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? ColorHex { get; set; }
    }
}
