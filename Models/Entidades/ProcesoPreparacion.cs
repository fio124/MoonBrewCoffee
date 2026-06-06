using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Models.Entidades
{
    public class ProcesoPreparacion
    {
        [Key]
        public int IdProceso { get; set; }

        public int IdProducto { get; set; }

        public int IdEstacion { get; set; }

        public int TiempoPreparacionMin { get; set; }

        public Producto? Producto { get; set; }

        public EstacionCocina? EstacionCocina { get; set; }

        public int Orden { get; set; }
    }
}