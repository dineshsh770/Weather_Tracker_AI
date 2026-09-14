using FirstReactApplication.Server.Models;
using System.Text.Json;

namespace FirstReactApplication.Server.Services;

public class OpenWeatherMapService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? string.Empty;
    }

    public async Task<WeatherDto> GetWeatherByCityAsync(string city)
    {
        var url = $"{BaseUrl}?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";
        return await FetchWeatherAsync(url);
    }

    public async Task<WeatherDto> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&units=metric&appid={_apiKey}";
        return await FetchWeatherAsync(url);
    }

    private async Task<WeatherDto> FetchWeatherAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(json, options);

        if (weatherResponse == null)
            throw new InvalidOperationException("Failed to deserialize weather data");

        return MapToDto(weatherResponse);
    }

    private WeatherDto MapToDto(WeatherResponse response)
    {
        return new WeatherDto
        {
            City = response.Name ?? "Unknown",
            Country = response.Sys?.Country ?? "Unknown",
            Temperature = Math.Round(response.Main?.Temp ?? 0, 1),
            FeelsLike = Math.Round(response.Main?.FeelsLike ?? 0, 1),
            Humidity = response.Main?.Humidity ?? 0,
            WindSpeed = Math.Round(response.Wind?.Speed ?? 0, 1),
            Description = response.Weather?.FirstOrDefault()?.Description ?? "N/A",
            Icon = response.Weather?.FirstOrDefault()?.Icon ?? "02d",
            TempMin = Math.Round(response.Main?.TempMin ?? 0, 1),
            TempMax = Math.Round(response.Main?.TempMax ?? 0, 1),
            Pressure = response.Main?.Pressure ?? 0,
            Visibility = response.Visibility,
            Sunrise = response.Sys?.Sunrise ?? 0,
            Sunset = response.Sys?.Sunset ?? 0
        };
    }
}
