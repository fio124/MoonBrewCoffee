using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public string? Image64 { get; set; }

        public int TiempoPreparacion { get; set; }

        public bool Activo { get; set; }

        public Categoria? Categoria { get; set; }
    }
}