using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;
using QuestPDF.Infrastructure;

namespace MoonBrewCoffee.Tests;

public class RegistroFacturaEImagenTests
{
    [Fact]
    public void Registro_RechazaContrasenaDebil()
    {
        var model = RegistroValido();
        model.Password = "12345678";
        model.ConfirmPassword = "12345678";

        var errors = Validate(model);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(model.Password)));
    }

    [Fact]
    public void Registro_AceptaDatosValidos()
    {
        Assert.Empty(Validate(RegistroValido()));
    }

    [Fact]
    public void FacturaPdf_GeneraDocumentoValido()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var pedido = new PedidoDTO
        {
            IdPedido = 15,
            FechaPedido = new DateTime(2026, 8, 22, 10, 30, 0),
            ClienteNombre = "Cliente Demo",
            ClienteCorreo = "cliente@moonbrew.cr",
            ClienteTelefono = "8888-8888",
            TipoEntrega = "Recogida en tienda",
            MetodoPago = "Efectivo",
            Estado = "Aceptada",
            Subtotal = 3500m,
            Impuesto = 455m,
            Total = 3955m,
            Detalles =
            [
                new PedidoDetalleDTO
                {
                    Tipo = "Producto",
                    Nombre = "Latte clásico",
                    PrecioUnitario = 3500m,
                    Cantidad = 1,
                    Subtotal = 3500m,
                    Impuesto = 455m
                }
            ]
        };

        var file = new FacturaPdfService().Generar(pedido, false);

        Assert.True(file.Length > 1000);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(file, 0, 4));
    }

    [Fact]
    public async Task Imagen_SeGuardaComoArchivoYDevuelveRuta()
    {
        var root = Path.Combine(Path.GetTempPath(), $"moonbrew-images-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);

        try
        {
            var environment = new TestEnvironment { WebRootPath = root };
            var service = new ServicioImagenes(environment);
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
            await using var stream = new MemoryStream(bytes);
            var formFile = new FormFile(stream, 0, bytes.Length, "imagenArchivo", "cafe.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var path = await service.GuardarAsync(formFile, "productos");
            var physicalPath = Path.Combine(root, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            Assert.StartsWith("/uploads/productos/", path);
            Assert.DoesNotContain("base64", path, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(physicalPath));

            service.EliminarSiEsLocal(path);
            Assert.False(File.Exists(physicalPath));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, true);
        }
    }

    private static RegistroViewModel RegistroValido() => new()
    {
        Nombre = "María",
        Apellido = "López",
        Correo = "maria@example.com",
        Telefono = "8888-8888",
        Password = "MoonBrew1",
        ConfirmPassword = "MoonBrew1"
    };

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "MoonBrewCoffee.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
