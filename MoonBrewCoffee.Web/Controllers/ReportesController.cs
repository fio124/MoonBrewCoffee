using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Web.Controllers;

public sealed class ReportesController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IPedidoReportPdfService _pdfService;

    public ReportesController(IPedidoService pedidoService, IPedidoReportPdfService pdfService)
    {
        _pedidoService = pedidoService;
        _pdfService = pdfService;
    }

    public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta, int? idEstado)
    {
        var periodo = NormalizarPeriodo(desde, hasta);
        var modelo = await CrearModelo(periodo.Desde, periodo.Hasta, idEstado);
        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> DescargarPedidosPdf(DateTime? desde, DateTime? hasta, int? idEstado)
    {
        var periodo = NormalizarPeriodo(desde, hasta);
        var modelo = await CrearModelo(periodo.Desde, periodo.Hasta, idEstado);
        var ingles = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";
        var archivo = _pdfService.Generar(modelo, ingles);
        var nombre = $"MoonBrew-Pedidos-{periodo.Desde:yyyyMMdd}-{periodo.Hasta:yyyyMMdd}.pdf";

        return File(archivo, "application/pdf", nombre);
    }

    private async Task<ReportesViewModel> CrearModelo(DateTime desde, DateTime hasta, int? idEstado)
    {
        var estados = await _pedidoService.GetStatusesAsync();
        var pedidos = await _pedidoService.GetAllAsync(desde, hasta, idEstado);
        return ReportesViewModel.Crear(desde, hasta, idEstado, estados, pedidos);
    }

    private static (DateTime Desde, DateTime Hasta) NormalizarPeriodo(DateTime? desde, DateTime? hasta)
    {
        var fechaHasta = (hasta ?? DateTime.Today).Date;
        var fechaDesde = (desde ?? new DateTime(fechaHasta.Year, fechaHasta.Month, 1)).Date;

        if (fechaDesde > fechaHasta)
            (fechaDesde, fechaHasta) = (fechaHasta, fechaDesde);

        return (fechaDesde, fechaHasta);
    }
}
