namespace MoonBrewCoffee.Web.Models
{
    public class CurrentUserViewModel
    {
        public int? IdUsuario { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Apellido { get; init; } = string.Empty;
        public string Correo { get; init; } = string.Empty;
        public string Telefono { get; init; } = string.Empty;
        public string Rol { get; init; } = string.Empty;
        public string NombreCompleto =>
            string.Join(" ", new[] { Nombre, Apellido }.Where(valor => !string.IsNullOrWhiteSpace(valor)));

        public bool EsCliente => Rol.Contains("cliente", StringComparison.OrdinalIgnoreCase);
        public bool EsAdministrador => Rol.Contains("admin", StringComparison.OrdinalIgnoreCase);
        public bool EsEncargado => Rol.Contains("encarg", StringComparison.OrdinalIgnoreCase);
    }
}
