using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Tests;

public class DefenseEvidenceTests
{
    [Fact]
    public void CartCount_SumsProductAndComboQuantities()
    {
        var cart = new CartViewModel
        {
            Lines = new List<CartLineViewModel>
            {
                new() { ItemType = "producto", ItemId = 1, UnitPrice = 2200m, Quantity = 2 },
                new() { ItemType = "combo", ItemId = 3, UnitPrice = 4500m, Quantity = 3 }
            }
        };

        Assert.Equal(5, cart.ItemCount);
        Assert.Equal(17900m, cart.Subtotal);
        Assert.Equal(2327m, cart.Tax);
        Assert.Equal(20227m, cart.Total);
    }

    [Fact]
    public void HomeDelivery_AddsConfiguredShippingCost()
    {
        var checkout = new CheckoutViewModel
        {
            DeliveryType = "domicilio",
            Cart = new CartViewModel
            {
                Lines = new List<CartLineViewModel>
                {
                    new() { ItemType = "producto", ItemId = 1, UnitPrice = 2000m, Quantity = 1 }
                }
            }
        };

        Assert.Equal(2500m, checkout.ShippingCost);
        Assert.Equal(checkout.Cart.Total + 2500m, checkout.GrandTotal);
    }

    [Theory]
    [InlineData("Cliente", true, false, false)]
    [InlineData("Administrador", false, true, false)]
    [InlineData("Encargado", false, false, true)]
    public void CurrentUser_ResolvesRolePermissions(
        string role,
        bool expectedClient,
        bool expectedAdministrator,
        bool expectedManager)
    {
        var current = new CurrentUserViewModel { Rol = role };

        Assert.Equal(expectedClient, current.EsCliente);
        Assert.Equal(expectedAdministrator, current.EsAdministrador);
        Assert.Equal(expectedManager, current.EsEncargado);
    }
}
