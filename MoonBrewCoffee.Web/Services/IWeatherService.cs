namespace MoonBrewCoffee.Web.Services
{
    public interface IWeatherService
    {
        Task<WeatherRecommendation?> GetSanJoseAsync(CancellationToken cancellationToken = default);
    }

    public record WeatherRecommendation(decimal Temperature, int WeatherCode, string Recommendation, bool IsFromCache);
}
