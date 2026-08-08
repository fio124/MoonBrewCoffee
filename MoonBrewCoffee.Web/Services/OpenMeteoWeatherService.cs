using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace MoonBrewCoffee.Web.Services
{
    public class OpenMeteoWeatherService : IWeatherService
    {
        private const string CacheKey = "weather:san-jose";
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public OpenMeteoWeatherService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<WeatherRecommendation?> GetSanJoseAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue<WeatherRecommendation>(CacheKey, out var cached) && cached is not null)
                return cached with { IsFromCache = true };

            try
            {
                var result = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(
                    "v1/forecast?latitude=9.9281&longitude=-84.0907&current=temperature_2m,weather_code&timezone=America%2FCosta_Rica",
                    cancellationToken);
                if (result?.Current is null) return null;

                var recommendation = new WeatherRecommendation(
                    result.Current.Temperature,
                    result.Current.WeatherCode,
                    result.Current.Temperature >= 24 ? "Un café frío va perfecto con este clima." : "Hoy combina muy bien con una bebida caliente.",
                    false);
                _cache.Set(CacheKey, recommendation, TimeSpan.FromMinutes(15));
                return recommendation;
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                return null;
            }
        }

        private sealed class OpenMeteoResponse
        {
            [JsonPropertyName("current")]
            public CurrentWeather? Current { get; set; }
        }

        private sealed class CurrentWeather
        {
            [JsonPropertyName("temperature_2m")]
            public decimal Temperature { get; set; }
            [JsonPropertyName("weather_code")]
            public int WeatherCode { get; set; }
        }
    }
}
