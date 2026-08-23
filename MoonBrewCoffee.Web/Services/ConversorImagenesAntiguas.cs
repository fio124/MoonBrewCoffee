using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;

namespace MoonBrewCoffee.Web.Services;

public static class ConversorImagenesAntiguas
{
    public static async Task ConvertirAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MoonBrewContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("ConversorImagenesAntiguas");

        try
        {
            var products = await context.Productos
                .Where(item => item.Image64 != null &&
                    !item.Image64.StartsWith("/") &&
                    !item.Image64.StartsWith("http") &&
                    !item.Image64.StartsWith("Imagenes/") &&
                    !item.Image64.StartsWith("images/") &&
                    !item.Image64.StartsWith("uploads/"))
                .ToListAsync();

            var combos = await context.Combos
                .Where(item => item.ImagenURL != null &&
                    !item.ImagenURL.StartsWith("/") &&
                    !item.ImagenURL.StartsWith("http") &&
                    !item.ImagenURL.StartsWith("Imagenes/") &&
                    !item.ImagenURL.StartsWith("images/") &&
                    !item.ImagenURL.StartsWith("uploads/"))
                .ToListAsync();

            foreach (var product in products)
            {
                var path = SaveBase64(environment.WebRootPath, "productos", product.Image64, product.IdProducto);
                if (path is not null)
                    product.Image64 = path;
            }

            foreach (var combo in combos)
            {
                var path = SaveBase64(environment.WebRootPath, "combos", combo.ImagenURL, combo.IdCombo);
                if (path is not null)
                    combo.ImagenURL = path;
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
                logger.LogInformation(
                    "Imágenes antiguas convertidas a archivos. Productos: {Products}; combos: {Combos}.",
                    products.Count,
                    combos.Count);
            }
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception,
                "No fue posible convertir las imágenes antiguas; la aplicación seguirá usando la compatibilidad Base64.");
        }
    }

    private static string? SaveBase64(string webRoot, string folder, string? source, int id)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        try
        {
            var encoded = source;
            var extension = ".jpg";
            if (source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var separator = source.IndexOf(',');
                if (separator < 0)
                    return null;

                var metadata = source[5..separator];
                extension = metadata.StartsWith("image/png", StringComparison.OrdinalIgnoreCase) ? ".png"
                    : metadata.StartsWith("image/webp", StringComparison.OrdinalIgnoreCase) ? ".webp"
                    : ".jpg";
                encoded = source[(separator + 1)..];
            }

            var bytes = Convert.FromBase64String(encoded);
            if (bytes.Length == 0)
                return null;

            extension = DetectExtension(bytes, extension);
            var directory = Path.Combine(webRoot, "uploads", folder);
            Directory.CreateDirectory(directory);
            var fileName = $"legacy-{id}-{Guid.NewGuid():N}{extension}";
            File.WriteAllBytes(Path.Combine(directory, fileName), bytes);
            return $"/uploads/{folder}/{fileName}";
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static string DetectExtension(byte[] bytes, string fallback)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return ".png";
        if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
            bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            return ".webp";
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return ".jpg";
        return fallback;
    }
}
