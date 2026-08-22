using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MoonBrewCoffee.Web.Formatting;
using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Web.Services;

public interface IPedidoReportPdfService
{
    byte[] Generar(ReportesViewModel reporte, bool ingles);
}

public sealed class PedidoReportPdfService : IPedidoReportPdfService
{
    private const string Cafe = "#42251B";
    private const string Caramelo = "#B77845";
    private const string Crema = "#F8F1E8";
    private const string Verde = "#247A50";

    public byte[] Generar(ReportesViewModel reporte, bool ingles)
    {
        var texto = new TextosReporte(ingles);

        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(32);
                pagina.DefaultTextStyle(estilo => estilo.FontFamily(Fonts.Arial).FontSize(9).FontColor(Cafe));

                pagina.Header().Element(contenedor => CrearEncabezado(contenedor, reporte, texto));
                pagina.Content().PaddingVertical(18).Column(columna =>
                {
                    columna.Spacing(18);
                    columna.Item().Element(contenedor => CrearResumen(contenedor, reporte, texto));
                    columna.Item().Text(texto.DetallePedidos).FontSize(15).Bold().FontColor(Cafe);
                    columna.Item().Element(contenedor => CrearTablaPedidos(contenedor, reporte, texto));

                    if (reporte.Productos.Count > 0)
                    {
                        columna.Item().PaddingTop(6).Text(texto.ProductosVendidos).FontSize(15).Bold().FontColor(Cafe);
                        columna.Item().Element(contenedor => CrearTablaProductos(contenedor, reporte, texto));
                    }
                });

                pagina.Footer()
                    .DefaultTextStyle(estilo => estilo.FontSize(8).FontColor(Colors.Grey.Medium))
                    .AlignCenter()
                    .Text(textoPagina =>
                {
                    textoPagina.Span(texto.Pagina + " ");
                    textoPagina.CurrentPageNumber();
                    textoPagina.Span(" / ");
                    textoPagina.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void CrearEncabezado(IContainer contenedor, ReportesViewModel reporte, TextosReporte texto)
    {
        contenedor.Background(Cafe).Padding(18).Row(fila =>
        {
            fila.RelativeItem().Column(columna =>
            {
                columna.Item().Text("MoonBrew Coffee").FontSize(21).Bold().FontColor(Colors.White);
                columna.Item().Text(texto.Titulo).FontSize(11).FontColor("#E8C8A8");
            });
            fila.ConstantItem(180).AlignRight().Column(columna =>
            {
                columna.Item().AlignRight().Text($"{reporte.Desde:dd/MM/yyyy} - {reporte.Hasta:dd/MM/yyyy}")
                    .Bold().FontColor(Colors.White);
                columna.Item().AlignRight().Text($"{texto.Generado}: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(8).FontColor("#E8C8A8");
            });
        });
    }

    private static void CrearResumen(IContainer contenedor, ReportesViewModel reporte, TextosReporte texto)
    {
        contenedor.Row(fila =>
        {
            ResumenItem(fila.RelativeItem(), texto.Pedidos, reporte.CantidadPedidos.ToString(), Cafe);
            fila.ConstantItem(10);
            ResumenItem(fila.RelativeItem(), texto.Subtotal, Moneda(reporte.Subtotal), Caramelo);
            fila.ConstantItem(10);
            ResumenItem(fila.RelativeItem(), texto.Impuestos, Moneda(reporte.Impuesto), "#805AD5");
            fila.ConstantItem(10);
            ResumenItem(fila.RelativeItem(), texto.Total, Moneda(reporte.TotalVentas), Verde);
        });
    }

    private static void ResumenItem(IContainer contenedor, string etiqueta, string valor, string color)
    {
        contenedor.Border(1).BorderColor("#E8DDD3").Background(Crema).Padding(10).Column(columna =>
        {
            columna.Item().Text(etiqueta).FontSize(8).FontColor(Colors.Grey.Darken1);
            columna.Item().PaddingTop(3).Text(valor).FontSize(13).Bold().FontColor(color);
        });
    }

    private static void CrearTablaPedidos(IContainer contenedor, ReportesViewModel reporte, TextosReporte texto)
    {
        contenedor.Table(tabla =>
        {
            tabla.ColumnsDefinition(columnas =>
            {
                columnas.ConstantColumn(38);
                columnas.ConstantColumn(66);
                columnas.RelativeColumn(1.4f);
                columnas.RelativeColumn();
                columnas.ConstantColumn(74);
            });

            tabla.Header(encabezado =>
            {
                CeldaEncabezado(encabezado.Cell(), texto.Numero);
                CeldaEncabezado(encabezado.Cell(), texto.Fecha);
                CeldaEncabezado(encabezado.Cell(), texto.Cliente);
                CeldaEncabezado(encabezado.Cell(), texto.Estado);
                CeldaEncabezado(encabezado.Cell(), texto.Total);
            });

            if (reporte.Pedidos.Count == 0)
            {
                tabla.Cell().ColumnSpan(5).BorderBottom(1).BorderColor("#E8DDD3").Padding(14)
                    .AlignCenter().Text(texto.SinResultados).Italic().FontColor(Colors.Grey.Medium);
                return;
            }

            foreach (var pedido in reporte.Pedidos)
            {
                Celda(tabla.Cell(), $"#{pedido.IdPedido}");
                Celda(tabla.Cell(), pedido.FechaPedido.ToString("dd/MM/yyyy"));
                Celda(tabla.Cell(), pedido.ClienteNombre);
                Celda(tabla.Cell(), pedido.Estado);
                Celda(tabla.Cell(), Moneda(pedido.Total), true);
            }
        });
    }

    private static void CrearTablaProductos(IContainer contenedor, ReportesViewModel reporte, TextosReporte texto)
    {
        contenedor.Table(tabla =>
        {
            tabla.ColumnsDefinition(columnas =>
            {
                columnas.RelativeColumn();
                columnas.ConstantColumn(85);
                columnas.ConstantColumn(95);
            });

            tabla.Header(encabezado =>
            {
                CeldaEncabezado(encabezado.Cell(), texto.Producto);
                CeldaEncabezado(encabezado.Cell(), texto.Cantidad);
                CeldaEncabezado(encabezado.Cell(), texto.Ventas);
            });

            foreach (var producto in reporte.Productos)
            {
                Celda(tabla.Cell(), producto.Nombre);
                Celda(tabla.Cell(), producto.Cantidad.ToString());
                Celda(tabla.Cell(), Moneda(producto.Total), true);
            }
        });
    }

    private static void CeldaEncabezado(IContainer contenedor, string valor) =>
        contenedor.Background(Cafe).PaddingVertical(8).PaddingHorizontal(7)
            .Text(valor).Bold().FontSize(8).FontColor(Colors.White);

    private static void Celda(IContainer contenedor, string valor, bool negrita = false)
    {
        var texto = contenedor.BorderBottom(1).BorderColor("#E8DDD3").PaddingVertical(8).PaddingHorizontal(7).Text(valor);
        if (negrita) texto.Bold().FontColor(Verde);
    }

    private static string Moneda(decimal valor) => $"₡{valor.ToColones()}";

    private sealed class TextosReporte(bool ingles)
    {
        public string Titulo => ingles ? "Order and sales report" : "Reporte de pedidos y ventas";
        public string Generado => ingles ? "Generated" : "Generado";
        public string Pedidos => ingles ? "Orders" : "Pedidos";
        public string Subtotal => ingles ? "Subtotal" : "Subtotal";
        public string Impuestos => ingles ? "Taxes" : "Impuestos";
        public string Total => ingles ? "Total" : "Total";
        public string DetallePedidos => ingles ? "Order details" : "Detalle de pedidos";
        public string ProductosVendidos => ingles ? "Products sold" : "Productos vendidos";
        public string Numero => ingles ? "No." : "N.º";
        public string Fecha => ingles ? "Date" : "Fecha";
        public string Cliente => ingles ? "Customer" : "Cliente";
        public string Estado => ingles ? "Status" : "Estado";
        public string SinResultados => ingles ? "No orders match the selected filters." : "No existen pedidos para los filtros seleccionados.";
        public string Producto => ingles ? "Product" : "Producto";
        public string Cantidad => ingles ? "Quantity" : "Cantidad";
        public string Ventas => ingles ? "Sales" : "Ventas";
        public string Pagina => ingles ? "Page" : "Página";
    }
}
