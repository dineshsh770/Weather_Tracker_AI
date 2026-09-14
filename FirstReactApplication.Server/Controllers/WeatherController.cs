using FirstReactApplication.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstReactApplication.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("city")]
    public async Task<IActionResult> GetWeatherByCity([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City name is required");

        try
        {
            var weather = await _weatherService.GetWeatherByCityAsync(city);
            return Ok(weather);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching weather for city: {City}", city);
            return NotFound("City not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching weather");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("coordinates")]
    public async Task<IActionResult> GetWeatherByCoordinates(
        [FromQuery] double latitude,
        [FromQuery] double longitude)
    {
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            return BadRequest("Invalid latitude/longitude");

        try
        {
            var weather = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude);
            return Ok(weather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather for coordinates");
            return StatusCode(500, "Internal server error");
        }
    }
}
