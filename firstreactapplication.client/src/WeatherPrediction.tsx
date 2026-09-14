import React, { useState, useEffect } from 'react';
import './WeatherPrediction.css';

interface WeatherAlert {
  type: string;
  severity: string;
  message: string;
  recommendation: string;
}

interface WeatherPredictionData {
  city: string;
  country: string;
  forecastText: string;
  alerts: WeatherAlert[];
  recommendations: string[];
  confidenceScore: number;
  generatedAt: string;
  nextUpdateTime: string;
}

interface WeatherPredictionProps {
  city: string;
  onDataReceived?: (data: WeatherPredictionData) => void;
}

const WeatherPrediction: React.FC<WeatherPredictionProps> = ({ city, onDataReceived }) => {
  const [prediction, setPrediction] = useState<WeatherPredictionData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (city) {
      fetchPrediction(city);
    }
  }, [city]);

  const fetchPrediction = async (cityName: string) => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(`/api/weather-prediction/analyze?city=${encodeURIComponent(cityName)}`);
      if (!response.ok) {
        throw new Error('Failed to fetch prediction');
      }
      const data: WeatherPredictionData = await response.json();
      setPrediction(data);
      onDataReceived?.(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch prediction');
      console.error('Prediction error:', err);
    } finally {
      setLoading(false);
    }
  };

  const getSeverityColor = (severity: string): string => {
    switch (severity.toLowerCase()) {
      case 'critical':
        return '#dc3545';
      case 'high':
        return '#ff6b6b';
      case 'medium':
        return '#ffc107';
      case 'low':
        return '#17a2b8';
      default:
        return '#6c757d';
    }
  };

  const getSeverityBgColor = (severity: string): string => {
    switch (severity.toLowerCase()) {
      case 'critical':
        return 'rgba(220, 53, 69, 0.1)';
      case 'high':
        return 'rgba(255, 107, 107, 0.1)';
      case 'medium':
        return 'rgba(255, 193, 7, 0.1)';
      case 'low':
        return 'rgba(23, 162, 184, 0.1)';
      default:
        return 'rgba(108, 117, 125, 0.1)';
    }
  };

  if (loading) {
    return (
      <div className="prediction-container">
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Analyzing weather patterns...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="prediction-container">
        <div className="error-message">
          <span>⚠️</span>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  if (!prediction) {
    return null;
  }

  return (
    <div className="prediction-container">
      {/* Forecast Card */}
      <div className="prediction-card forecast-card">
        <div className="card-header">
          <h3>🔮 AI Weather Forecast</h3>
          <span className="confidence-badge">
            Confidence: {prediction.confidenceScore}%
          </span>
        </div>
        <div className="card-content">
          <p className="forecast-text">{prediction.forecastText}</p>
          <div className="timestamp">
            Generated: {new Date(prediction.generatedAt).toLocaleTimeString()}
          </div>
        </div>
      </div>

      {/* Alerts Section */}
      {prediction.alerts.length > 0 && (
        <div className="prediction-card alerts-card">
          <div className="card-header">
            <h3>⚠️ Weather Alerts ({prediction.alerts.length})</h3>
          </div>
          <div className="alerts-list">
            {prediction.alerts.map((alert, index) => (
              <div
                key={index}
                className="alert-item"
                style={{ backgroundColor: getSeverityBgColor(alert.severity) }}
              >
                <div className="alert-header">
                  <span
                    className="severity-badge"
                    style={{ backgroundColor: getSeverityColor(alert.severity), color: 'white' }}
                  >
                    {alert.severity}
                  </span>
                  <span className="alert-type">{alert.type}</span>
                </div>
                <p className="alert-message">{alert.message}</p>
                <div className="alert-recommendation">
                  <strong>👉 Action:</strong> {alert.recommendation}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Recommendations Section */}
      {prediction.recommendations.length > 0 && (
        <div className="prediction-card recommendations-card">
          <div className="card-header">
            <h3>💡 Recommendations</h3>
          </div>
          <ul className="recommendations-list">
            {prediction.recommendations.map((rec, index) => (
              <li key={index}>
                <span className="recommendation-icon">✓</span>
                {rec}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Info Footer */}
      <div className="prediction-footer">
        <p className="disclaimer">
          🤖 This AI prediction is based on current weather data from OpenWeatherMap.
          Always check official weather alerts for your region.
        </p>
      </div>
    </div>
  );
};

export default WeatherPrediction;
