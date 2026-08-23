using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Web.Formatting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MoonBrewCoffee.Web.Services;

public interface IFacturaPdfService
{
    byte[] Generar(PedidoDTO pedido, bool ingles);
}

public sealed class FacturaPdfService : IFacturaPdfService
{
    private const string Cafe = "#42251B";
    private const string Caramelo = "#B77845";
    private const string Crema = "#FAF5EF";
    private const string Borde = "#E7D9CC";

    public byte[] Generar(PedidoDTO pedido, bool ingles)
    {
        var t = new Textos(ingles);

        return Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4);
            pagina.Margin(30);
            pagina.DefaultTextStyle(estilo => estilo.FontFamily(Fonts.Arial).FontSize(9).FontColor(Cafe));

            pagina.Header().Background(Cafe).Padding(18).Row(fila =>
            {
                fila.RelativeItem().Column(columna =>
                {
                    columna.Item().Text("MoonBrew Coffee").FontSize(21).Bold().FontColor(Colors.White);
                    columna.Item().Text(t.Factura).FontSize(10).FontColor("#E8C8A8");
                });
                fila.ConstantItem(150).AlignRight().Column(columna =>
                {
                    columna.Item().AlignRight().Text($"#{pedido.IdPedido}").FontSize(20).Bold().FontColor(Colors.White);
                    columna.Item().AlignRight().Text(pedido.FechaPedido.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor("#E8C8A8");
                });
            });

            pagina.Content().PaddingVertical(18).Column(columna =>
            {
                columna.Spacing(16);
                columna.Item().Element(c => Informacion(c, pedido, t));
                columna.Item().Text(t.Detalle).FontSize(14).Bold();
                columna.Item().Element(c => Tabla(c, pedido, t));
                columna.Item().AlignRight().Width(250).Element(c => Totales(c, pedido, t));
                columna.Item().PaddingTop(8).BorderTop(1).BorderColor(Borde).PaddingTop(10)
                    .AlignCenter().Text(t.Gracias).Italic().FontColor(Caramelo);
            });

            pagina.Footer()
                .DefaultTextStyle(estilo => estilo.FontSize(8).FontColor(Colors.Grey.Medium))
                .AlignCenter()
                .Text(texto =>
                {
                    texto.Span(t.Pagina + " ");
                    texto.CurrentPageNumber();
                    texto.Span(" / ");
                    texto.TotalPages();
                });
        })).GeneratePdf();
    }

    private static void Informacion(IContainer contenedor, PedidoDTO pedido, Textos t)
    {
        contenedor.Border(1).BorderColor(Borde).Background(Crema).Padding(12).Column(columna =>
        {
            columna.Spacing(8);
            columna.Item().Row(fila =>
            {
                Dato(fila.RelativeItem(), t.Cliente, pedido.ClienteNombre);
                Dato(fila.RelativeItem(), t.Estado, pedido.Estado);
                Dato(fila.RelativeItem(), t.Entrega, pedido.TipoEntrega);
            });
            columna.Item().Row(fila =>
            {
                Dato(fila.RelativeItem(), t.Correo, pedido.ClienteCorreo);
                Dato(fila.RelativeItem(), t.Telefono, pedido.ClienteTelefono);
                Dato(fila.RelativeItem(), t.Pago, pedido.MetodoPago);
            });

            if (!string.IsNullOrWhiteSpace(pedido.DireccionEntrega))
                columna.Item().Text($"{t.Direccion}: {pedido.DireccionEntrega}").FontSize(8);
        });
    }

    private static void Dato(IContainer contenedor, string etiqueta, string valor)
    {
        contenedor.PaddingRight(8).Column(columna =>
        {
            columna.Item().Text(etiqueta).FontSize(7).Bold().FontColor(Caramelo);
            columna.Item().Text(string.IsNullOrWhiteSpace(valor) ? "-" : valor).Bold();
        });
    }

    private static void Tabla(IContainer contenedor, PedidoDTO pedido, Textos t)
    {
        contenedor.Table(tabla =>
        {
            tabla.ColumnsDefinition(columnas =>
            {
                columnas.RelativeColumn(2.2f);
                columnas.ConstantColumn(62);
                columnas.ConstantColumn(48);
                columnas.ConstantColumn(72);
                columnas.ConstantColumn(72);
            });

            tabla.Header(encabezado =>
            {
                Encabezado(encabezado.Cell(), t.Producto);
                Encabezado(encabezado.Cell(), t.Precio);
                Encabezado(encabezado.Cell(), t.Cantidad);
                Encabezado(encabezado.Cell(), t.Impuesto);
                Encabezado(encabezado.Cell(), t.Subtotal);
            });

            foreach (var linea in pedido.Detalles)
            {
                Celda(tabla.Cell(), $"{linea.Nombre}\n{linea.Tipo}");
                Celda(tabla.Cell(), Moneda(linea.PrecioUnitario));
                Celda(tabla.Cell(), linea.Cantidad.ToString());
                Celda(tabla.Cell(), Moneda(linea.Impuesto));
                Celda(tabla.Cell(), Moneda(linea.Subtotal), true);

                if (!string.IsNullOrWhiteSpace(linea.Observaciones))
                {
                    tabla.Cell().ColumnSpan(5).BorderBottom(1).BorderColor(Borde).Padding(6)
                        .Text($"{t.Observaciones}: {linea.Observaciones}").FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                }
            }
        });
    }

    private static void Totales(IContainer contenedor, PedidoDTO pedido, Textos t)
    {
        contenedor.Border(1).BorderColor(Borde).Padding(12).Column(columna =>
        {
            Total(columna.Item(), t.Subtotal, pedido.Subtotal);
            Total(columna.Item(), t.Impuesto, pedido.Impuesto);
            Total(columna.Item(), t.Envio, pedido.CostoEnvio);
            columna.Item().PaddingTop(7).BorderTop(2).BorderColor(Caramelo).PaddingTop(7).Row(fila =>
            {
                fila.RelativeItem().Text(t.Total).Bold().FontSize(11);
                fila.AutoItem().Text(Moneda(pedido.Total)).Bold().FontSize(14).FontColor(Caramelo);
            });
        });
    }

    private static void Total(IContainer contenedor, string etiqueta, decimal valor) =>
        contenedor.PaddingVertical(3).Row(fila =>
        {
            fila.RelativeItem().Text(etiqueta);
            fila.AutoItem().Text(Moneda(valor)).Bold();
        });

    private static void Encabezado(IContainer contenedor, string texto) =>
        contenedor.Background(Cafe).Padding(7).Text(texto).Bold().FontSize(7).FontColor(Colors.White);

    private static void Celda(IContainer contenedor, string texto, bool negrita = false)
    {
        var descriptor = contenedor.BorderBottom(1).BorderColor(Borde).Padding(7).Text(texto);
        if (negrita)
            descriptor.Bold();
    }

    private static string Moneda(decimal valor) => $"₡{valor.ToColones()}";

    private sealed class Textos(bool ingles)
    {
        public string Factura => ingles ? "Order invoice" : "Factura de pedido";
        public string Cliente => ingles ? "CUSTOMER" : "CLIENTE";
        public string Estado => ingles ? "STATUS" : "ESTADO";
        public string Entrega => ingles ? "DELIVERY" : "ENTREGA";
        public string Correo => ingles ? "EMAIL" : "CORREO";
        public string Telefono => ingles ? "PHONE" : "TELÉFONO";
        public string Pago => ingles ? "PAYMENT" : "PAGO";
        public string Direccion => ingles ? "Delivery address" : "Dirección de entrega";
        public string Detalle => ingles ? "Order details" : "Detalle del pedido";
        public string Producto => ingles ? "Product or combo" : "Producto o combo";
        public string Precio => ingles ? "Price" : "Precio";
        public string Cantidad => ingles ? "Qty." : "Cant.";
        public string Impuesto => ingles ? "Tax" : "Impuesto";
        public string Subtotal => "Subtotal";
        public string Observaciones => ingles ? "Notes" : "Observaciones";
        public string Envio => ingles ? "Shipping" : "Envío";
        public string Total => "Total";
        public string Gracias => ingles ? "Thank you for choosing MoonBrew Coffee." : "Gracias por elegir MoonBrew Coffee.";
        public string Pagina => ingles ? "Page" : "Página";
    }
}
