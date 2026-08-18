using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Application.DTOs
{
    public class ProductoDTO : IValidatableObject
    {
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "1", "99999999", ErrorMessage = "El precio debe ser mayor que ₡0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Seleccione una imagen para el producto.")]
        public string? Image64 { get; set; }

        [Range(1, 1440, ErrorMessage = "El tiempo debe estar entre 1 y 1440 minutos.")]
        public int TiempoPreparacion { get; set; }

        public bool Activo { get; set; }

        public string NombreCategoria { get; set; } = string.Empty;
        public CategoriaDTO? Categoria { get; set; }
        public List<int> IngredientesSeleccionados { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IngredientesSeleccionados.Count == 0)
            {
                yield return new ValidationResult(
                    "Seleccione al menos un ingrediente.",
                    new[] { nameof(IngredientesSeleccionados) });
            }
        }
    }
}
