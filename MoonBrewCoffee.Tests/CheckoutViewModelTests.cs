using System.ComponentModel.DataAnnotations;
using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Tests;

public class ValidacionPagoTests
{
    [Fact]
    public void EntregaDomicilio_RequiereDireccion()
    {
        var model = ValidCardCheckout();
        model.DeliveryType = "domicilio";
        model.DeliveryAddress = null;

        Assert.Contains(Validate(model), error => error.MemberNames.Contains(nameof(model.DeliveryAddress)));
    }

    [Fact]
    public void PagoEfectivo_RequiereMontoSuficiente()
    {
        var model = ValidCardCheckout();
        model.PaymentMethod = "efectivo";
        model.CashReceived = model.GrandTotal - 1;

        Assert.Contains(Validate(model), error => error.MemberNames.Contains(nameof(model.CashReceived)));
    }

    [Fact]
    public void PedidoDeEncargado_RequiereCliente()
    {
        var model = ValidCardCheckout();
        model.CanSelectClient = true;
        model.ClientId = null;

        Assert.Contains(Validate(model), error => error.MemberNames.Contains(nameof(model.ClientId)));
    }

    [Fact]
    public void PagoTarjeta_RechazaDatosInvalidos()
    {
        var model = ValidCardCheckout();
        model.CardNumber = "123";
        model.CardExpiry = "20/99";
        model.CardCvv = "1";

        var errors = Validate(model);
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(model.CardNumber)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(model.CardExpiry)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(model.CardCvv)));
    }

    [Fact]
    public void PagoValido_NoPresentaErrores()
    {
        Assert.Empty(Validate(ValidCardCheckout()));
    }

    private static CheckoutViewModel ValidCardCheckout() => new()
    {
        OperationKey = Guid.NewGuid().ToString("N"),
        DeliveryType = "tienda",
        PaymentMethod = "credito",
        CardholderName = "Cliente MoonBrew",
        CardNumber = "4111111111111111",
        CardExpiry = "12/30",
        CardCvv = "123"
    };

    private static List<ValidationResult> Validate(CheckoutViewModel model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }
}
