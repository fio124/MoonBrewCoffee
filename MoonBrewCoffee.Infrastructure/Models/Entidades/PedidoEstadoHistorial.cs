using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class PedidoEstadoHistorial
    {
        [Key]
        public int IdHistorial { get; set; }
        public int IdPedido { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaCambio { get; set; }
        public int? IdUsuario { get; set; }

        public Pedido? Pedido { get; set; }
        public EstadoPedido? Estado { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
