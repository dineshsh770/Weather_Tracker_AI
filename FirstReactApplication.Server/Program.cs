using FirstReactApplication.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register weather services
builder.Services.AddHttpClient<IWeatherService, OpenWeatherMapService>();
builder.Services.AddHttpClient<IWeatherPredictionService, WeatherPredictionService>();
builder.Services.AddScoped<IWeatherPredictionService, WeatherPredictionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Map fallback AFTER controllers so API routes are handled first
app.MapFallbackToFile("/index.html");

app.Run();
