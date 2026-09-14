import React, { useState, useEffect } from 'react';
import './Weather.css';
import WeatherPrediction from './WeatherPrediction';

interface WeatherData {
  city: string;
  country: string;
  temperature: number;
  feelsLike: number;
  humidity: number;
  windSpeed: number;
  description: string;
  icon: string;
  tempMin: number;
  tempMax: number;
  pressure: number;
  visibility: number;
  sunrise: number;
  sunset: number;
}

const Weather: React.FC = () => {
  const [weather, setWeather] = useState<WeatherData | null>(null);
  const [city, setCity] = useState('Hyderabad');
  const [inputValue, setInputValue] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [geolocationSupported, setGeolocationSupported] = useState(true);

  const fetchWeather = async (cityName: string) => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(
        `/api/weather/city?city=${encodeURIComponent(cityName)}`
      );
      if (!response.ok) {
        throw new Error('City not found');
      }
      const data: WeatherData = await response.json();
      setWeather(data);
      setCity(cityName);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch weather');
    } finally {
      setLoading(false);
    }
  };

  const fetchWeatherByCoordinates = async (latitude: number, longitude: number) => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(
        `/api/weather/coordinates?latitude=${latitude}&longitude=${longitude}`
      );
      if (!response.ok) {
        throw new Error('Failed to fetch weather');
      }
      const data: WeatherData = await response.json();
      setWeather(data);
      setCity(data.city);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch weather');
    } finally {
      setLoading(false);
    }
  };

  const handleGetGeolocation = () => {
    if (!navigator.geolocation) {
      setGeolocationSupported(false);
      return;
    }

    setLoading(true);
    navigator.geolocation.getCurrentPosition(
      (position) => {
        fetchWeatherByCoordinates(
          position.coords.latitude,
          position.coords.longitude
        );
      },
      (err) => {
        setError('Failed to get your location: ' + err.message);
        setLoading(false);
      }
    );
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (inputValue.trim()) {
      fetchWeather(inputValue);
      setInputValue('');
    }
  };

  useEffect(() => {
    fetchWeather(city);
  }, []);

  const getWeatherIcon = (iconCode: string) => {
    return `https://openweathermap.org/img/wn/${iconCode}@4x.png`;
  };

  const formatTime = (timestamp: number) => {
    return new Date(timestamp * 1000).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });
  };

  return (
    <div className="weather-container">
      <div className="weather-header">
        <h1>🌤️ Weather App</h1>
        <p>Real-time weather updates powered by OpenWeatherMap</p>
      </div>

      <div className="search-section">
        <form onSubmit={handleSearch}>
          <div className="search-box">
            <input
              type="text"
              placeholder="Enter city name (e.g., Tokyo, Paris, New York)..."
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              className="search-input"
            />
            <button type="submit" className="search-btn">
              Search
            </button>
          </div>
        </form>

        <button
          onClick={handleGetGeolocation}
          className="location-btn"
          disabled={loading || !geolocationSupported}
        >
          📍 Use My Location
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      {loading && <div className="loading">Loading weather data...</div>}

      {weather && !loading && (
        <div className="weather-content">
          <WeatherPrediction city={city} />
          <div className="main-weather-card">
            <div className="location">
              <h2>{weather.city}, {weather.country}</h2>
            </div>

            <div className="weather-display">
              <img
                src={getWeatherIcon(weather.icon)}
                alt={weather.description}
                className="weather-icon"
              />
              <div className="temperature-section">
                <div className="current-temp">{weather.temperature}°C</div>
                <div className="description">{weather.description}</div>
                <div className="feels-like">
                  Feels like {weather.feelsLike}°C
                </div>
              </div>
            </div>

            <div className="weather-grid">
              <div className="weather-item">
                <span className="label">Min Temp</span>
                <span className="value">{weather.tempMin}°C</span>
              </div>
              <div className="weather-item">
                <span className="label">Max Temp</span>
                <span className="value">{weather.tempMax}°C</span>
              </div>
              <div className="weather-item">
                <span className="label">Humidity</span>
                <span className="value">{weather.humidity}%</span>
              </div>
              <div className="weather-item">
                <span className="label">Wind Speed</span>
                <span className="value">{weather.windSpeed} m/s</span>
              </div>
              <div className="weather-item">
                <span className="label">Pressure</span>
                <span className="value">{weather.pressure} hPa</span>
              </div>
              <div className="weather-item">
                <span className="label">Visibility</span>
                <span className="value">{(weather.visibility / 1000).toFixed(1)} km</span>
              </div>
              <div className="weather-item">
                <span className="label">Sunrise</span>
                <span className="value">{formatTime(weather.sunrise)}</span>
              </div>
              <div className="weather-item">
                <span className="label">Sunset</span>
                <span className="value">{formatTime(weather.sunset)}</span>
              </div>
            </div>
          </div>
        </div>
      )}

      {!loading && !weather && !error && (
        <div className="no-data">
          <p>Enter a city name or use your location to see weather information</p>
        </div>
      )}
    </div>
  );
};

export default Weather;
