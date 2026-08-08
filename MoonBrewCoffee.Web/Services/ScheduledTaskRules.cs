using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Web.Services
{
    public static class ScheduledTaskRules
    {
        public static bool IsMenuAvailable(Menu menu, DateTime localNow)
        {
            if (!menu.Activo || localNow.Date < menu.FechaInicio.Date || localNow.Date > menu.FechaFin.Date)
                return false;

            var currentTime = localNow.TimeOfDay;
            if (menu.HoraInicio <= menu.HoraFin)
                return currentTime >= menu.HoraInicio && currentTime <= menu.HoraFin;

            return currentTime >= menu.HoraInicio || currentTime <= menu.HoraFin;
        }

        public static decimal ApplyDiscount(decimal currentPrice, decimal discountPercent, decimal minimumPrice)
        {
            var safePercent = Math.Clamp(discountPercent, 0m, 100m);
            var discounted = decimal.Round(currentPrice * (1m - safePercent / 100m), 2, MidpointRounding.AwayFromZero);
            return Math.Max(minimumPrice, discounted);
        }

        public static decimal ApplyIncrease(decimal currentPrice, decimal increase)
        {
            return decimal.Round(currentPrice + Math.Max(0m, increase), 2, MidpointRounding.AwayFromZero);
        }

        public static string BuildExecutionKey(string taskName, DateTimeOffset utcNow, TimeSpan interval)
        {
            var seconds = Math.Max(1L, (long)interval.TotalSeconds);
            var slot = utcNow.ToUnixTimeSeconds() / seconds;
            return $"{taskName}:{slot}";
        }
    }
}
