using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class Producto
    {
       
        [Key]
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImageURL { get; set; }
        public int TiempoPreparacion { get; set; }
        public bool Activo { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
