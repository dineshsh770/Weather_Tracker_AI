namespace FirstReactApplication.Server.Models;

public class WeatherPrediction
{
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string ForecastText { get; set; }
    public List<WeatherAlert> Alerts { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];
    public int ConfidenceScore { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime NextUpdateTime { get; set; }
}
