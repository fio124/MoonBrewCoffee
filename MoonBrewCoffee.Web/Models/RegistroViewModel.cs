using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Web.Models;

public sealed class RegistroViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 60 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9+()\-\s]{8,20}$", ErrorMessage = "Ingresa un número de teléfono válido.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$",
        ErrorMessage = "Usa al menos 8 caracteres, una mayúscula, una minúscula y un número.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
