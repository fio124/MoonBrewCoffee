namespace MoonBrewCoffee.Application.DTOs
{
    public class RolDTO
    {
        public int IdRol { get; set; }

        public string Nombre { get; set; } = "";

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}