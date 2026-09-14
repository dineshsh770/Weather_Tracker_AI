using FirstReactApplication.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstReactApplication.Server.Controllers;

[ApiController]
[Route("api/weather-prediction")]
public class WeatherPredictionController : ControllerBase
{
    private readonly IWeatherPredictionService _predictionService;
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherPredictionController> _logger;

    public WeatherPredictionController(
        IWeatherPredictionService predictionService,
        IWeatherService weatherService,
        ILogger<WeatherPredictionController> logger)
    {
        _predictionService = predictionService;
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("analyze")]
    public async Task<IActionResult> AnalyzeWeather([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City name is required");

        try
        {
            var weather = await _weatherService.GetWeatherByCityAsync(city);
            var prediction = await _predictionService.AnalyzeWeatherAsync(city, weather);
            return Ok(prediction);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error analyzing weather for city: {City}", city);
            return NotFound("City not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error analyzing weather");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City name is required");

        try
        {
            var weather = await _weatherService.GetWeatherByCityAsync(city);
            var alerts = await _predictionService.GetAlertsAsync(city, weather);
            return Ok(new { city, alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts for city: {City}", city);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City name is required");

        try
        {
            var weather = await _weatherService.GetWeatherByCityAsync(city);
            var recommendations = await _predictionService.GetRecommendationsAsync(city, weather);
            return Ok(new { city, recommendations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for city: {City}", city);
            return StatusCode(500, "Internal server error");
        }
    }
}
