using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Application.DTOs
{
    public class ProcesoPreparacionDTO
    {
        public int IdProceso { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto.")]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una estación.")]
        public int IdEstacion { get; set; }

        [Range(1, 1440, ErrorMessage = "El tiempo debe estar entre 1 y 1440 minutos.")]
        public int TiempoPreparacionMin { get; set; }

        [Range(1, 100, ErrorMessage = "El orden debe estar entre 1 y 100.")]
        public int Orden { get; set; }

        public string NombreProducto { get; set; } = string.Empty;
        public string NombreEstacion { get; set; } = string.Empty;
    }
}
