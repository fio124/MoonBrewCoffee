using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Tests;

public class ReglasTareasProgramadasTests
{
    [Fact]
    public void MenuDisponible_EsVerdaderoDentroDelHorario()
    {
        var menu = CreateMenu();

        var result = ScheduledTaskRules.IsMenuAvailable(menu, new DateTime(2026, 8, 8, 15, 30, 0));

        Assert.True(result);
    }

    [Theory]
    [InlineData("2026-07-31T15:30:00")]
    [InlineData("2026-08-08T11:59:00")]
    [InlineData("2026-08-08T18:01:00")]
    public void MenuDisponible_EsFalsoFueraDelHorario(string date)
    {
        var menu = CreateMenu();

        var result = ScheduledTaskRules.IsMenuAvailable(menu, DateTime.Parse(date));

        Assert.False(result);
    }

    [Fact]
    public void AplicarDescuento_RespetaPrecioMinimo()
    {
        var result = ScheduledTaskRules.ApplyDiscount(1510m, 5m, 1500m);

        Assert.Equal(1500m, result);
    }

    [Fact]
    public void AplicarDescuento_RedondeaADosDecimales()
    {
        var result = ScheduledTaskRules.ApplyDiscount(3501m, 5m, 1500m);

        Assert.Equal(3325.95m, result);
    }

    [Fact]
    public void AplicarAumento_SoloAgregaMontosNoNegativos()
    {
        Assert.Equal(3510m, ScheduledTaskRules.ApplyIncrease(3500m, 10m));
        Assert.Equal(3500m, ScheduledTaskRules.ApplyIncrease(3500m, -10m));
    }

    [Fact]
    public void CrearClaveEjecucion_SeMantieneEnElMismoIntervalo()
    {
        var first = ScheduledTaskRules.BuildExecutionKey(
            "TardeandoDiscount",
            DateTimeOffset.FromUnixTimeSeconds(100),
            TimeSpan.FromSeconds(15));
        var second = ScheduledTaskRules.BuildExecutionKey(
            "TardeandoDiscount",
            DateTimeOffset.FromUnixTimeSeconds(104),
            TimeSpan.FromSeconds(15));

        Assert.Equal(first, second);
    }

    private static Menu CreateMenu()
    {
        return new Menu
        {
            Activo = true,
            FechaInicio = new DateTime(2026, 8, 1),
            FechaFin = new DateTime(2026, 8, 31),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFin = new TimeSpan(18, 0, 0)
        };
    }
}
