using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Tests;

public class ScheduledTaskRulesTests
{
    [Fact]
    public void IsMenuAvailable_ReturnsTrueInsideDateAndTimeRange()
    {
        var menu = CreateMenu();

        var result = ScheduledTaskRules.IsMenuAvailable(menu, new DateTime(2026, 8, 8, 15, 30, 0));

        Assert.True(result);
    }

    [Theory]
    [InlineData("2026-07-31T15:30:00")]
    [InlineData("2026-08-08T11:59:00")]
    [InlineData("2026-08-08T18:01:00")]
    public void IsMenuAvailable_ReturnsFalseOutsideSchedule(string date)
    {
        var menu = CreateMenu();

        var result = ScheduledTaskRules.IsMenuAvailable(menu, DateTime.Parse(date));

        Assert.False(result);
    }

    [Fact]
    public void ApplyDiscount_RespectsConfiguredMinimum()
    {
        var result = ScheduledTaskRules.ApplyDiscount(1510m, 5m, 1500m);

        Assert.Equal(1500m, result);
    }

    [Fact]
    public void ApplyDiscount_RoundsCurrencyToTwoDecimals()
    {
        var result = ScheduledTaskRules.ApplyDiscount(3501m, 5m, 1500m);

        Assert.Equal(3325.95m, result);
    }

    [Fact]
    public void ApplyIncrease_AddsOnlyNonNegativeAmount()
    {
        Assert.Equal(3510m, ScheduledTaskRules.ApplyIncrease(3500m, 10m));
        Assert.Equal(3500m, ScheduledTaskRules.ApplyIncrease(3500m, -10m));
    }

    [Fact]
    public void BuildExecutionKey_IsStableInsideSameInterval()
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
