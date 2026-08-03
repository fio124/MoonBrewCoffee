using Microsoft.AspNetCore.Mvc;

namespace MoonBrewCoffee.Web.Controllers;

internal static class StoredImageResult
{
    public static IActionResult Create(ControllerBase controller, string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return controller.NotFound();

        if (Uri.TryCreate(source, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return controller.Redirect(source);
        }

        // También se admiten rutas que ya estaban guardadas en la base de datos.
        if (source.StartsWith("~/", StringComparison.Ordinal))
            return controller.Redirect(source[1..]);

        if (source.StartsWith('/') ||
            source.StartsWith("Imagenes/", StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return controller.Redirect('/' + source.TrimStart('/'));
        }

        try
        {
            if (source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var separator = source.IndexOf(',');
                if (separator < 0)
                    return controller.NotFound();

                var metadata = source[5..separator];
                var contentType = metadata.Split(';')[0];

                if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return controller.NotFound();

                var bytes = Convert.FromBase64String(source[(separator + 1)..]);
                return controller.File(bytes, contentType);
            }

            return controller.File(Convert.FromBase64String(source), "image/jpeg");
        }
        catch (FormatException)
        {
            return controller.NotFound();
        }
    }
}
