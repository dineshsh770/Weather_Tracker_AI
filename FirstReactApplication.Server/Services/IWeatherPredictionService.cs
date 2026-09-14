using FirstReactApplication.Server.Models;

namespace FirstReactApplication.Server.Services;

public interface IWeatherPredictionService
{
    Task<WeatherPrediction> AnalyzeWeatherAsync(string city, WeatherDto currentWeather);
    Task<List<WeatherAlert>> GetAlertsAsync(string city, WeatherDto currentWeather);
    Task<List<string>> GetRecommendationsAsync(string city, WeatherDto currentWeather);
}
