namespace MoonBrewCoffee.Application.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string PasswordHash { get; set; } = "";

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }

        public string NombreRol { get; set; } = string.Empty;
    }
}