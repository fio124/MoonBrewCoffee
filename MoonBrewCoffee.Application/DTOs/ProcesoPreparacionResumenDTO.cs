namespace MoonBrewCoffee.Application.DTOs
{
    public class ProcesoPreparacionResumenDTO
    {
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; } = "";

        public int CantidadPasos { get; set; }

        public int TiempoTotal { get; set; }
    }
}