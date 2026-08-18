using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Application.DTOs
{
    public class MenuDTO : IValidatableObject
    {
        public int IdMenu { get; set; }

        [Required(ErrorMessage = "El nombre del menú es obligatorio.")]
        [StringLength(
            100,
            ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha final es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }
            = new TimeSpan(6, 0, 0);

        [Required(ErrorMessage = "La hora final es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFin { get; set; }
            = new TimeSpan(22, 0, 0);

        public bool Disponible { get; set; } = true;

        public bool Activo { get; set; } = true;
        public bool VigenteAhora { get; set; }

        public List<int> ProductosSeleccionados { get; set; } = new();

        public List<int> CombosSeleccionados { get; set; } = new();
        public List<ProductoDTO> Productos { get; set; } = new();
        public List<ComboDTO> Combos { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (FechaInicio.Date > FechaFin.Date)
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser mayor que la fecha final.",
                    new[]
                    {
                        nameof(FechaInicio),
                        nameof(FechaFin)
                    });
            }

            if (HoraInicio >= HoraFin)
            {
                yield return new ValidationResult(
                    "La hora de inicio debe ser menor que la hora final.",
                    new[]
                    {
                        nameof(HoraInicio),
                        nameof(HoraFin)
                    });
            }

            if (ProductosSeleccionados.Count == 0 &&
                CombosSeleccionados.Count == 0)
            {
                yield return new ValidationResult(
                    "El menú debe incluir al menos un producto o un combo.");
            }
        }
    }
}