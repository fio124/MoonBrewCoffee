using Microsoft.AspNetCore.Http;

namespace MoonBrewCoffee.Web.Services;

public interface IServicioImagenes
{
    Task<string> GuardarAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    void EliminarSiEsLocal(string? imagePath);
}

public sealed class ServicioImagenes : IServicioImagenes
{
    private const long MaximumFileSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly string _webRootPath;

    public ServicioImagenes(IWebHostEnvironment environment)
    {
        _webRootPath = environment.WebRootPath;
    }

    public async Task<string> GuardarAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            throw new ArgumentException("La imagen seleccionada está vacía.");
        if (file.Length > MaximumFileSize)
            throw new ArgumentException("La imagen no puede superar los 5 MB.");
        if (string.IsNullOrWhiteSpace(file.ContentType) ||
            !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Seleccione un archivo de imagen válido.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException("Solo se permiten imágenes JPG, PNG o WEBP.");

        var safeFolder = folder.Equals("productos", StringComparison.OrdinalIgnoreCase)
            ? "productos"
            : folder.Equals("combos", StringComparison.OrdinalIgnoreCase)
                ? "combos"
                : throw new ArgumentException("La carpeta de imágenes no es válida.");

        var directory = Path.Combine(_webRootPath, "uploads", safeFolder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(directory, fileName);
        await using var stream = new FileStream(
            destination,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{safeFolder}/{fileName}";
    }

    public void EliminarSiEsLocal(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) ||
            !imagePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return;

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var candidate = Path.GetFullPath(Path.Combine(_webRootPath, relativePath));
        var uploadsRoot = Path.GetFullPath(Path.Combine(_webRootPath, "uploads")) + Path.DirectorySeparatorChar;

        if (candidate.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(candidate))
            File.Delete(candidate);
    }
}
