using FirstReactApplication.Server.Models;

namespace FirstReactApplication.Server.Services;

public interface IWeatherService
{
    Task<WeatherDto> GetWeatherByCityAsync(string city);
    Task<WeatherDto> GetWeatherByCoordinatesAsync(double latitude, double longitude);
}
