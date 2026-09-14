namespace FirstReactApplication.Server.Models;

public class WeatherDto
{
    public required string City { get; set; }
    public required string Country { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public int Pressure { get; set; }
    public int Visibility { get; set; }
    public long Sunrise { get; set; }
    public long Sunset { get; set; }
}
