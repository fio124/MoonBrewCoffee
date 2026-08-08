using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Web.Models
{
    public class CheckoutViewModel : IValidatableObject
    {
        [Required]
        public string OperationKey { get; set; } = string.Empty;
        public CartViewModel Cart { get; set; } = new();
        public CurrentUserViewModel CurrentUser { get; set; } = new();
        public bool CanSelectClient { get; set; }
        public List<SelectListItem> Clients { get; set; } = new();

        [Display(Name = "Cliente")]
        public int? ClientId { get; set; }

        [Required(ErrorMessage = "Selecciona un método de entrega.")]
        [Display(Name = "Método de entrega")]
        public string DeliveryType { get; set; } = "tienda";

        [StringLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
        [Display(Name = "Dirección de entrega")]
        public string? DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Selecciona un método de pago.")]
        [Display(Name = "Método de pago")]
        public string PaymentMethod { get; set; } = "credito";

        [Display(Name = "Nombre del titular")]
        public string? CardholderName { get; set; }

        [Display(Name = "Número de tarjeta")]
        public string? CardNumber { get; set; }

        [Display(Name = "Vencimiento")]
        public string? CardExpiry { get; set; }

        [Display(Name = "CVV")]
        public string? CardCvv { get; set; }

        [Display(Name = "Monto recibido")]
        public decimal? CashReceived { get; set; }

        public decimal ShippingCost => DeliveryType == "domicilio" ? 2500m : 0m;
        public decimal GrandTotal => Cart.Total + ShippingCost;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CanSelectClient && !ClientId.HasValue)
                yield return new ValidationResult("Selecciona el cliente que solicita el pedido.", new[] { nameof(ClientId) });

            if (DeliveryType == "domicilio" && string.IsNullOrWhiteSpace(DeliveryAddress))
                yield return new ValidationResult("Ingresa la dirección para la entrega a domicilio.", new[] { nameof(DeliveryAddress) });

            if (PaymentMethod is "credito" or "debito")
            {
                if (string.IsNullOrWhiteSpace(CardholderName))
                    yield return new ValidationResult("Ingresa el nombre del titular.", new[] { nameof(CardholderName) });
                var digits = new string((CardNumber ?? string.Empty).Where(char.IsDigit).ToArray());
                if (digits.Length is < 13 or > 19)
                    yield return new ValidationResult("Ingresa un número de tarjeta válido.", new[] { nameof(CardNumber) });
                if (string.IsNullOrWhiteSpace(CardExpiry) || !System.Text.RegularExpressions.Regex.IsMatch(CardExpiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                    yield return new ValidationResult("Usa el formato MM/AA para el vencimiento.", new[] { nameof(CardExpiry) });
                if (string.IsNullOrWhiteSpace(CardCvv) || !System.Text.RegularExpressions.Regex.IsMatch(CardCvv, @"^\d{3,4}$"))
                    yield return new ValidationResult("Ingresa un CVV válido.", new[] { nameof(CardCvv) });
            }

            if (PaymentMethod == "efectivo" && (!CashReceived.HasValue || CashReceived.Value < GrandTotal))
                yield return new ValidationResult("El monto recibido debe cubrir el total del pedido.", new[] { nameof(CashReceived) });
        }
    }

    public class OrderHistoryViewModel
    {
        public List<PedidoDTO> Orders { get; set; } = new();
        public List<EstadoPedidoDTO> Statuses { get; set; } = new();
        public bool IsManagementView { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? StatusId { get; set; }
    }
}
