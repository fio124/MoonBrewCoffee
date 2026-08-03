namespace MoonBrewCoffee.Application.DTOs
{
    public class MenuDisponibleDTO
    {
        public int IdMenu { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public List<CategoriaMenuDTO> Categorias { get; set; } = new();

        public List<ComboDTO> Combos { get; set; } = new();
    }

    public class CategoriaMenuDTO
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public List<ProductoDTO> Productos { get; set; } = new();
    }
}