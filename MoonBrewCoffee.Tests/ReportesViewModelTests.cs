using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;
using QuestPDF.Infrastructure;
using System.Text;

namespace MoonBrewCoffee.Tests;

public class ReportesViewModelTests
{
    [Fact]
    public void Crear_CalculaTotalesDePedidos()
    {
        var pedidos = new[]
        {
            Pedido(1, 1000m, 130m, 0m, 1130m),
            Pedido(2, 2000m, 260m, 500m, 2760m)
        };

        var reporte = ReportesViewModel.Crear(
            new DateTime(2026, 8, 1),
            new DateTime(2026, 8, 31),
            null,
            [],
            pedidos);

        Assert.Equal(2, reporte.CantidadPedidos);
        Assert.Equal(3000m, reporte.Subtotal);
        Assert.Equal(390m, reporte.Impuesto);
        Assert.Equal(500m, reporte.Envios);
        Assert.Equal(3890m, reporte.TotalVentas);
    }

    [Fact]
    public void Crear_AgrupaProductosPorNombre()
    {
        var pedido = Pedido(1, 5000m, 650m, 0m, 5650m);
        pedido.Detalles =
        [
            new PedidoDetalleDTO { Tipo = "Producto", Nombre = "Latte", Cantidad = 2, Subtotal = 3000m },
            new PedidoDetalleDTO { Tipo = "Producto", Nombre = "latte", Cantidad = 1, Subtotal = 1500m },
            new PedidoDetalleDTO { Tipo = "Combo", Nombre = "Combo dulce", Cantidad = 1, Subtotal = 500m }
        ];

        var reporte = ReportesViewModel.Crear(
            DateTime.Today,
            DateTime.Today,
            null,
            [],
            [pedido]);

        var producto = Assert.Single(reporte.Productos);
        Assert.Equal("Latte", producto.Nombre);
        Assert.Equal(3, producto.Cantidad);
        Assert.Equal(4500m, producto.Total);
    }

    [Fact]
    public void GenerarPdf_CreaUnArchivoPdfValido()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var reporte = ReportesViewModel.Crear(
            new DateTime(2026, 8, 1),
            new DateTime(2026, 8, 31),
            null,
            [],
            [Pedido(1, 3500m, 455m, 0m, 3955m)]);

        var archivo = new PedidoReportPdfService().Generar(reporte, false);

        Assert.True(archivo.Length > 1000);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(archivo, 0, 4));
    }

    private static PedidoDTO Pedido(int id, decimal subtotal, decimal impuesto, decimal envio, decimal total) => new()
    {
        IdPedido = id,
        Subtotal = subtotal,
        Impuesto = impuesto,
        CostoEnvio = envio,
        Total = total
    };
}
