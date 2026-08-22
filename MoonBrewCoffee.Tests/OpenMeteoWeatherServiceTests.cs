using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Tests;

public class ServicioClimaTests
{
    [Fact]
    public async Task RespuestaClima_SeTransformaYGuardaEnCache()
    {
        var handler = new StubHandler("{\"current\":{\"temperature_2m\":27.4,\"weather_code\":2}}");
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.open-meteo.com/") };
        var service = new OpenMeteoWeatherService(client, new MemoryCache(new MemoryCacheOptions()));

        var first = await service.GetSanJoseAsync();
        var second = await service.GetSanJoseAsync();

        Assert.NotNull(first);
        Assert.Equal(27.4m, first.Temperature);
        Assert.Contains("café frío", first.Recommendation);
        Assert.True(second!.IsFromCache);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task FalloDeRed_NoInterrumpePaginaInicio()
    {
        var client = new HttpClient(new StubHandler(null)) { BaseAddress = new Uri("https://api.open-meteo.com/") };
        var service = new OpenMeteoWeatherService(client, new MemoryCache(new MemoryCacheOptions()));

        Assert.Null(await service.GetSanJoseAsync());
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string? _json;
        public int RequestCount { get; private set; }
        public StubHandler(string? json) => _json = json;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            if (_json is null) throw new HttpRequestException("Simulated outage");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            });
        }
    }
}
