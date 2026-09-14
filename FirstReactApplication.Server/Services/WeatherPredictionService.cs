using FirstReactApplication.Server.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace FirstReactApplication.Server.Services;

public class WeatherPredictionService : IWeatherPredictionService
{
    private readonly string _geminiApiKey;
    private readonly ILogger<WeatherPredictionService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _httpClient;
    private const string GeminiModel = "gemini-1.5-flash";
    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    public WeatherPredictionService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WeatherPredictionService> logger,
        IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _geminiApiKey = configuration["Google:ApiKey"] ?? string.Empty;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<WeatherPrediction> AnalyzeWeatherAsync(string city, WeatherDto currentWeather)
    {
        var cacheKey = $"weather_prediction_{city}";

        if (_memoryCache.TryGetValue(cacheKey, out WeatherPrediction? cachedPrediction))
        {
            _logger.LogInformation("Returning cached prediction for {City}", city);
            return cachedPrediction!;
        }

        if (string.IsNullOrEmpty(_geminiApiKey))
        {
            _logger.LogWarning("Gemini API key not configured, returning demo prediction");
            return GetDemoPrediction(city, currentWeather);
        }

        try
        {
            var alerts = await GetAlertsAsync(city, currentWeather);
            var recommendations = await GetRecommendationsAsync(city, currentWeather);
            var forecastText = await GetForecastAnalysisAsync(city, currentWeather);

            var prediction = new WeatherPrediction
            {
                City = city,
                Country = currentWeather.Country,
                ForecastText = forecastText,
                Alerts = alerts,
                Recommendations = recommendations,
                ConfidenceScore = CalculateConfidenceScore(currentWeather),
                GeneratedAt = DateTime.UtcNow,
                NextUpdateTime = DateTime.UtcNow.AddHours(1)
            };

            _memoryCache.Set(cacheKey, prediction, TimeSpan.FromHours(1));
            return prediction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing weather for {City}, returning demo", city);
            return GetDemoPrediction(city, currentWeather);
        }
    }

    public async Task<List<WeatherAlert>> GetAlertsAsync(string city, WeatherDto currentWeather)
    {
        if (string.IsNullOrEmpty(_geminiApiKey))
            return GetDemoAlerts(currentWeather);

        var prompt = $@"Analyze the following weather conditions and identify any weather alerts or warnings.
Weather for {city}:
- Current Temperature: {currentWeather.Temperature}°C
- Feels Like: {currentWeather.FeelsLike}°C
- Temperature Range: {currentWeather.TempMin}°C to {currentWeather.TempMax}°C
- Humidity: {currentWeather.Humidity}%
- Wind Speed: {currentWeather.WindSpeed} m/s
- Weather: {currentWeather.Description}
- Pressure: {currentWeather.Pressure} hPa
- Visibility: {currentWeather.Visibility}m

Based on these conditions, identify any alerts. Return a JSON array with objects containing:
- type: (Storm, Heat, Cold, Wind, Humidity, Visibility, etc.)
- severity: (Low, Medium, High, Critical)
- message: Brief alert message
- recommendation: What people should do

Return ONLY valid JSON array, no markdown or explanation.";

        try
        {
            var alertsJson = await CallGeminiAPIAsync(prompt);
            var alerts = JsonSerializer.Deserialize<List<WeatherAlert>>(alertsJson);
            return alerts ?? GetDemoAlerts(currentWeather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts from Gemini");
            return GetDemoAlerts(currentWeather);
        }
    }

    public async Task<List<string>> GetRecommendationsAsync(string city, WeatherDto currentWeather)
    {
        if (string.IsNullOrEmpty(_geminiApiKey))
            return GetDemoRecommendations(currentWeather);

        var prompt = $@"Based on the following weather conditions in {city}, provide practical activity recommendations for people.
Weather:
- Temperature: {currentWeather.Temperature}°C
- Humidity: {currentWeather.Humidity}%
- Wind Speed: {currentWeather.WindSpeed} m/s
- Weather: {currentWeather.Description}
- Visibility: {currentWeather.Visibility}m

Provide 5 specific recommendations about:
1. What to wear
2. Outdoor activities to do/avoid
3. Health precautions
4. Travel tips

Return a JSON array of strings (recommendations only), no markdown.";

        try
        {
            var recsJson = await CallGeminiAPIAsync(prompt);
            var recommendations = JsonSerializer.Deserialize<List<string>>(recsJson);
            return recommendations ?? GetDemoRecommendations(currentWeather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations from Gemini");
            return GetDemoRecommendations(currentWeather);
        }
    }

    private async Task<string> GetForecastAnalysisAsync(string city, WeatherDto currentWeather)
    {
        if (string.IsNullOrEmpty(_geminiApiKey))
            return GetDemoForecast(currentWeather);

        var prompt = $@"Analyze the current weather conditions for {city} and provide a brief weather forecast analysis.
Current Conditions:
- Temperature: {currentWeather.Temperature}°C (Feels like {currentWeather.FeelsLike}°C)
- Min/Max: {currentWeather.TempMin}°C / {currentWeather.TempMax}°C
- Humidity: {currentWeather.Humidity}%
- Wind: {currentWeather.WindSpeed} m/s
- Pressure: {currentWeather.Pressure} hPa
- Conditions: {currentWeather.Description}

Provide a 2-3 sentence weather forecast prediction based on these metrics. Be concise and practical.";

        try
        {
            return await CallGeminiAPIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast from Gemini");
            return GetDemoForecast(currentWeather);
        }
    }

    private async Task<string> CallGeminiAPIAsync(string userPrompt)
    {
        try
        {
            var url = $"{GeminiApiUrl}/{GeminiModel}:generateContent?key={_geminiApiKey}";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = userPrompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            var textContent = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return textContent.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            return string.Empty;
        }
    }

    private int CalculateConfidenceScore(WeatherDto weather)
    {
        int confidence = 85;

        if (weather.Visibility < 1000)
            confidence -= 10;

        if (Math.Abs(weather.FeelsLike - weather.Temperature) > 5)
            confidence -= 5;

        return Math.Max(50, Math.Min(100, confidence));
    }

    private WeatherPrediction GetDemoPrediction(string city, WeatherDto weather)
    {
        return new WeatherPrediction
        {
            City = city,
            Country = weather.Country,
            ForecastText = GetDemoForecast(weather),
            Alerts = GetDemoAlerts(weather),
            Recommendations = GetDemoRecommendations(weather),
            ConfidenceScore = 75,
            GeneratedAt = DateTime.UtcNow,
            NextUpdateTime = DateTime.UtcNow.AddHours(1)
        };
    }

    private string GetDemoForecast(WeatherDto weather)
    {
        return weather.Temperature switch
        {
            > 30 => "Expect continued warm conditions with no significant changes. Stay hydrated and seek shade during peak hours.",
            > 20 => "Generally pleasant weather with stable conditions. Good day for outdoor activities.",
            > 10 => "Mild conditions expected. A jacket may be needed, especially in the evening.",
            _ => "Cold conditions expected. Bundle up and limit outdoor exposure time."
        };
    }

    private List<WeatherAlert> GetDemoAlerts(WeatherDto weather)
    {
        var alerts = new List<WeatherAlert>();

        if (weather.Temperature > 35)
            alerts.Add(new WeatherAlert
            {
                Type = "Heat",
                Severity = "High",
                Message = "Extreme heat warning - temperatures exceed 35°C",
                Recommendation = "Stay indoors during peak heat, drink plenty of water, avoid strenuous activities"
            });

        if (weather.Temperature < 0)
            alerts.Add(new WeatherAlert
            {
                Type = "Cold",
                Severity = "High",
                Message = "Freezing conditions - temperatures below 0°C",
                Recommendation = "Wear protective clothing, limit outdoor time, watch for ice on roads"
            });

        if (weather.WindSpeed > 10)
            alerts.Add(new WeatherAlert
            {
                Type = "Wind",
                Severity = "Medium",
                Message = $"Strong winds - {weather.WindSpeed} m/s",
                Recommendation = "Secure loose objects, use caution when driving"
            });

        if (weather.Humidity > 80 && weather.Temperature > 25)
            alerts.Add(new WeatherAlert
            {
                Type = "Humidity",
                Severity = "Medium",
                Message = "High humidity with warm temperature - feels oppressive",
                Recommendation = "Stay hydrated, limit heavy exertion, seek air-conditioned areas"
            });

        if (weather.Visibility < 1000)
            alerts.Add(new WeatherAlert
            {
                Type = "Visibility",
                Severity = "Medium",
                Message = $"Low visibility - {weather.Visibility}m",
                Recommendation = "Drive with caution, use headlights, reduce speed"
            });

        return alerts;
    }

    private List<string> GetDemoRecommendations(WeatherDto weather)
    {
        var recommendations = new List<string>();

        if (weather.Temperature < 10)
        {
            recommendations.Add("Wear warm layers: thermal underwear, sweater, and heavy coat");
            recommendations.Add("Don't forget a hat, gloves, and scarf to protect extremities");
        }
        else if (weather.Temperature > 25)
        {
            recommendations.Add("Wear light, breathable clothing (cotton or linen)");
            recommendations.Add("Use sunscreen SPF 30+ and wear sunglasses");
        }
        else
        {
            recommendations.Add("A light jacket should be sufficient");
        }

        if (weather.Humidity > 70)
            recommendations.Add("Bring a light rain jacket - humid air often precedes storms");

        if (weather.WindSpeed > 5)
            recommendations.Add("Secure any outdoor items and use caution in open areas");

        recommendations.Add("Check UV index and stay hydrated throughout the day");

        return recommendations;
    }
}
