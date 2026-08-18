using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Application.DTOs
{
    public class ComboDTO : IValidatableObject
    {
        public int IdCombo { get; set; }

        [Required(ErrorMessage = "El nombre del combo es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "1", "99999999", ErrorMessage = "El precio debe ser mayor que ₡0.")]
        public decimal PrecioCombo { get; set; }

        public string? ImagenURL { get; set; }
        public bool Activo { get; set; }
        public List<int> ProductosSeleccionados { get; set; } = new();
        public List<ProductoDTO> Productos { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductosSeleccionados.Count == 0)
            {
                yield return new ValidationResult(
                    "Seleccione al menos un producto para el combo.",
                    new[] { nameof(ProductosSeleccionados) });
            }
        }
    }
}
