using System.Globalization;
using System.Text;

namespace MoonBrewCoffee.Web.Localization
{
    public static class OrderTranslationKeys
    {
        public static string? Status(string? value) => Normalize(value) switch
        {
            "pendiente" or "pending" => "orders.statusPending",
            "aceptada" or "aceptado" or "accepted" => "orders.statusAccepted",
            "preparacion" or "preparation" => "orders.statusPreparation",
            "procesando" or "processing" => "orders.statusProcessing",
            "entregada" or "entregado" or "delivered" => "orders.statusDelivered",
            _ => null
        };

        public static string? Delivery(string? value) => Normalize(value) switch
        {
            "tienda" or "retiro en tienda" or "retiro en la tienda" or "recogida en tienda" or "store pickup" => "orders.storePickup",
            "domicilio" or "entrega a domicilio" or "home delivery" => "orders.homeDelivery",
            _ => null
        };

        public static string? Payment(string? value) => Normalize(value) switch
        {
            "efectivo" or "cash" => "orders.cash",
            "credito" or "tarjeta de credito" or "credit" or "credit card" => "orders.creditCardValue",
            "debito" or "tarjeta de debito" or "debit" or "debit card" => "orders.debitCardValue",
            _ => null
        };

        public static string? ItemType(string? value) => Normalize(value) switch
        {
            "producto" or "product" => "orders.product",
            "combo" => "orders.combo",
            _ => null
        };

        public static string? Responsible(string? value) => Normalize(value) switch
        {
            "sistema moonbrew" or "moonbrew system" => "orders.systemUser",
            _ => null
        };

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposed.Length);
            foreach (var character in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(char.ToLowerInvariant(character));
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
