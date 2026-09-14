# 🌤️ Weather API Reference

## Base URL
```
https://localhost:7274/api
```

---

## Weather Endpoints (Existing)

### Get Current Weather by City
```http
GET /weather/city?city=London
```

**Parameters:**
- `city` (required): City name (string)

**Response (200 OK):**
```json
{
  "city": "London",
  "country": "GB",
  "temperature": 15.5,
  "feelsLike": 14.2,
  "humidity": 72,
  "windSpeed": 3.5,
  "description": "partly cloudy",
  "icon": "02d",
  "tempMin": 13.1,
  "tempMax": 17.8,
  "pressure": 1013,
  "visibility": 10000,
  "sunrise": 1693468800,
  "sunset": 1693520000
}
```

---

### Get Weather by GPS Coordinates
```http
GET /weather/coordinates?latitude=51.5074&longitude=-0.1278
```

**Parameters:**
- `latitude` (required): Latitude (-90 to 90)
- `longitude` (required): Longitude (-180 to 180)

**Response (200 OK):**
```json
{
  "city": "London",
  "country": "GB",
  "temperature": 15.5,
  ...
}
```

**Error Responses:**
- `400 Bad Request` - Invalid latitude/longitude
- `500 Internal Server Error` - Server error

---

## Weather Prediction Endpoints (New)

### Get Full Weather Analysis
```http
GET /weather-prediction/analyze?city=London
```

**Description:**
Performs complete weather analysis including forecast, alerts, and recommendations.

**Parameters:**
- `city` (required): City name (string)

**Response (200 OK):**
```json
{
  "city": "London",
  "country": "GB",
  "forecastText": "Generally pleasant weather with stable conditions. Good day for outdoor activities.",
  "alerts": [
    {
      "type": "Wind",
      "severity": "Medium",
      "message": "Strong winds - 12.5 m/s",
      "recommendation": "Secure loose objects, use caution when driving"
    }
  ],
  "recommendations": [
    "Wear light, breathable clothing (cotton or linen)",
    "Use sunscreen SPF 30+ and wear sunglasses",
    "Bring a light rain jacket - humid air often precedes storms",
    "Secure any outdoor items and use caution in open areas",
    "Check UV index and stay hydrated throughout the day"
  ],
  "confidenceScore": 87,
  "generatedAt": "2024-09-09T10:30:00Z",
  "nextUpdateTime": "2024-09-09T11:30:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - City name missing
- `404 Not Found` - City not found
- `500 Internal Server Error` - Server error

**Performance:**
- Cached responses: <10ms
- First request: 1-3 seconds (includes Claude API call)

---

### Get Weather Alerts Only
```http
GET /weather-prediction/alerts?city=London
```

**Description:**
Gets only weather alerts detected by AI analysis.

**Parameters:**
- `city` (required): City name (string)

**Response (200 OK):**
```json
{
  "city": "London",
  "alerts": [
    {
      "type": "Heat",
      "severity": "High",
      "message": "Extreme heat warning - temperatures exceed 35°C",
      "recommendation": "Stay indoors during peak heat, drink plenty of water, avoid strenuous activities"
    },
    {
      "type": "Humidity",
      "severity": "Medium",
      "message": "High humidity with warm temperature - feels oppressive",
      "recommendation": "Stay hydrated, limit heavy exertion, seek air-conditioned areas"
    }
  ]
}
```

**Alert Types:**
- `Storm` - Severe weather
- `Heat` - Extreme temperatures (>35°C)
- `Cold` - Freezing conditions (<0°C)
- `Wind` - High wind speeds (>10 m/s)
- `Humidity` - Oppressive humidity + heat
- `Visibility` - Low visibility conditions

**Severity Levels:**
- `Critical` - Immediate action required
- `High` - Take precautions
- `Medium` - Be aware of conditions
- `Low` - Minor concern

---

### Get Activity Recommendations Only
```http
GET /weather-prediction/recommendations?city=London
```

**Description:**
Gets only activity recommendations based on current weather.

**Parameters:**
- `city` (required): City name (string)

**Response (200 OK):**
```json
{
  "city": "London",
  "recommendations": [
    "Wear light, breathable clothing (cotton or linen)",
    "Use sunscreen SPF 30+ and wear sunglasses",
    "Good day for outdoor activities",
    "Stay hydrated - drink water regularly",
    "Avoid strenuous activities during peak heat (12-3pm)"
  ]
}
```

---

## Data Models

### WeatherPrediction
```typescript
{
  city: string;                    // City name
  country: string;                 // Country code (ISO 3166-1 alpha-2)
  forecastText: string;            // AI-generated forecast (2-3 sentences)
  alerts: WeatherAlert[];          // Array of detected alerts
  recommendations: string[];       // Array of activity recommendations
  confidenceScore: number;         // 0-100 prediction reliability
  generatedAt: DateTime;           // When prediction was created
  nextUpdateTime: DateTime;        // When prediction becomes stale
}
```

### WeatherAlert
```typescript
{
  type: string;           // Alert type (Storm, Heat, Cold, Wind, etc.)
  severity: string;       // Critical, High, Medium, Low
  message: string;        // Alert description
  recommendation: string; // Recommended action
}
```

### WeatherDto (Current Weather)
```typescript
{
  city: string;
  country: string;
  temperature: number;      // Celsius
  feelsLike: number;       // Celsius
  humidity: number;        // 0-100%
  windSpeed: number;       // m/s
  description: string;     // Weather description
  icon: string;            // Weather icon code
  tempMin: number;         // Celsius
  tempMax: number;         // Celsius
  pressure: number;        // hPa
  visibility: number;      // meters
  sunrise: number;         // Unix timestamp
  sunset: number;          // Unix timestamp
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "error": "City name is required"
}
```

### 404 Not Found
```json
{
  "error": "City not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "Internal server error"
}
```

---

## Common Use Cases

### 1. Display Current Weather + Prediction
```bash
# Get current weather
GET /weather/city?city=London

# Get full prediction
GET /weather-prediction/analyze?city=London

# Combine results for display
```

### 2. Alert-Only View
```bash
GET /weather-prediction/alerts?city=London
```

### 3. Recommendations-Only View
```bash
GET /weather-prediction/recommendations?city=London
```

### 4. Background Polling
```bash
# Poll every 60 minutes
GET /weather-prediction/analyze?city=London

# Results cached for 1 hour
# Subsequent requests return instantly
```

### 5. Multi-City Dashboard
```bash
GET /weather/city?city=London
GET /weather/city?city=Paris
GET /weather/city?city=Tokyo

GET /weather-prediction/analyze?city=London
GET /weather-prediction/analyze?city=Paris
GET /weather-prediction/analyze?city=Tokyo
```

---

## Performance Characteristics

| Scenario | Time | Notes |
|----------|------|-------|
| Cached prediction | <10ms | Same city within 1 hour |
| First prediction | 1-3s | Claude API call + cache |
| Invalid city | 200ms | OpenWeatherMap lookup |
| Alerts query | <10ms | If cached |
| Recommendations query | <10ms | If cached |

---

## Caching Strategy

**Current Implementation:**
- Predictions cached for **1 hour**
- Cache key: `weather_prediction_{city}`
- Auto-expiration after timeout
- Manual invalidation available (future)

**Cache Hit Scenarios:**
- Same city searched within 1 hour → Instant response
- Different city → New API call + cached result
- After 1 hour → Fresh prediction generated

---

## Rate Limiting

**Current Limits:**
- No built-in rate limiting
- OpenWeatherMap: 1,000 calls/day (free tier)
- Claude API: Based on account limits
- Recommended: 1 call per user per hour

**Future Implementation:**
- Per-IP rate limiting
- Per-user API key limiting
- Sliding window rate limiting

---

## Authentication

**Current Implementation:**
- No authentication required
- CORS enabled for all origins
- No API key needed for frontend

**Production Recommendations:**
- Implement JWT authentication
- Add API key for public endpoints
- Restrict CORS to known domains
- Add request signing

---

## Example cURL Commands

### Get Current Weather
```bash
curl -X GET "https://localhost:7274/api/weather/city?city=London" \
  -H "Content-Type: application/json"
```

### Get Full Prediction
```bash
curl -X GET "https://localhost:7274/api/weather-prediction/analyze?city=London" \
  -H "Content-Type: application/json"
```

### Get Alerts
```bash
curl -X GET "https://localhost:7274/api/weather-prediction/alerts?city=London" \
  -H "Content-Type: application/json"
```

### Get Recommendations
```bash
curl -X GET "https://localhost:7274/api/weather-prediction/recommendations?city=London" \
  -H "Content-Type: application/json"
```

---

## Example JavaScript/Fetch

```javascript
// Get prediction
async function getWeatherPrediction(city) {
  try {
    const response = await fetch(
      `/api/weather-prediction/analyze?city=${encodeURIComponent(city)}`
    );
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching prediction:', error);
  }
}

// Usage
const prediction = await getWeatherPrediction('London');
console.log(prediction.forecastText);
```

---

## Swagger/OpenAPI

**Available at Development:**
```
https://localhost:7274/swagger
https://localhost:7274/swagger/v1/swagger.json
```

Interactive API documentation with try-it-out capability.

---

## Version Info

- **API Version**: 1.0
- **Date**: September 9, 2024
- **Framework**: ASP.NET Core 8
- **Weather Source**: OpenWeatherMap v2.5
- **AI Model**: Claude 3.5 Sonnet (Anthropic)

---

## Support

For API issues:
1. Check error response message
2. Verify city name spelling
3. Check browser console for network errors
4. Review backend logs: `dotnet run` terminal
5. Consult full documentation in `AI_WEATHER_AGENT_SETUP.md`

---

## Changelog

### Version 1.0 (September 9, 2024)
- ✅ Added `/weather-prediction/analyze` endpoint
- ✅ Added `/weather-prediction/alerts` endpoint
- ✅ Added `/weather-prediction/recommendations` endpoint
- ✅ Implemented Claude AI integration
- ✅ Added prediction caching (1 hour)
- ✅ Confidence scoring
- ✅ Demo mode fallback
- ✅ Comprehensive error handling

---

## Future API Endpoints (Planned)

```
# Forecast endpoints
GET /weather-prediction/forecast-5day?city=London
GET /weather-prediction/forecast-hourly?city=London

# Subscription endpoints
POST /weather-prediction/subscribe
DELETE /weather-prediction/unsubscribe

# Analytics endpoints
GET /weather-prediction/accuracy?city=London&days=30
GET /weather-prediction/stats?period=month

# Admin endpoints
DELETE /weather-prediction/cache?city=London
GET /weather-prediction/health
```

---

Happy coding! 🌤️🤖
