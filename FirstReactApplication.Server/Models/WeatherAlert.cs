namespace FirstReactApplication.Server.Models;

public class WeatherAlert
{
    public required string Type { get; set; }
    public required string Severity { get; set; }
    public required string Message { get; set; }
    public required string Recommendation { get; set; }
}
