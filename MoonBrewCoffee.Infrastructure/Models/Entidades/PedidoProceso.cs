using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class PedidoProceso
    {
        [Key]
        public int IdPedidoProceso { get; set; }
        public int IdPedido { get; set; }
        public int IdEstacion { get; set; }
        public int Orden { get; set; }
        [MaxLength(180)]
        public string Descripcion { get; set; } = string.Empty;
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdEncargado { get; set; }

        public Pedido? Pedido { get; set; }
        public EstacionCocina? Estacion { get; set; }
        public Usuario? Encargado { get; set; }
    }
}
