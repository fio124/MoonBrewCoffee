using System.Globalization;

namespace MoonBrewCoffee.Web.Formatting;

public static class PriceFormattingExtensions
{
    private static readonly NumberFormatInfo ColonesFormat = new()
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ",",
        NumberDecimalDigits = 0
    };

    public static string ToColones(this decimal value)
    {
        return value.ToString("N0", ColonesFormat);
    }
}
