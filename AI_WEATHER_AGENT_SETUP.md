# 🤖 AI Weather Prediction Agent Setup Guide

## Overview

Your weather app now includes an **AI-powered Weather Prediction Agent** built with Claude AI (Anthropic). This agent provides:

✅ **AI Weather Forecasts** - Smart 3-5 day trend predictions  
✅ **Smart Alerts** - Automatically detects severe weather warnings  
✅ **Activity Recommendations** - What to wear, activities to do/avoid  
✅ **Proactive Insights** - Contextual advice based on current conditions  

---

## Architecture Overview

```
React Frontend
    ↓ (User searches city)
WeatherPredictionController
    ↓
1. OpenWeatherMapService (get current weather)
    ↓
2. WeatherPredictionService (analyze + call Claude)
    ↓
3. Claude AI (Anthropic API)
    - Analyzes weather patterns
    - Generates forecast
    - Detects alerts
    - Creates recommendations
    ↓
4. Cache Layer (1-hour prediction caching)
    ↓
Frontend Display
    - Shows forecast card
    - Displays alerts with severity
    - Lists recommendations
```

---

## Getting Started

### Step 1: Get Your Anthropic API Key

**Option A: Quick Start (Free Trial)**
1. Go to https://console.anthropic.com
2. Sign up with email
3. Navigate to **API Keys** → **Create Key**
4. Copy the key (starts with `sk-ant-`)
5. You get $5 in free credits to test

**Option B: Production Setup**
- Get paid API key after free credits run out
- Pricing: ~$3 per 1M input tokens, ~$15 per 1M output tokens
- For weather predictions: ~500-1000 tokens per request = ~$0.01-0.03 per request

### Step 2: Add API Key to Configuration

Open `appsettings.Development.json`:

```bash
cd FirstReactApplication
code appsettings.Development.json
```

Add your Anthropic API key:

```json
{
  "Logging": {...},
  "OpenWeatherMap": {...},
  "Anthropic": {
    "ApiKey": "sk-ant-YOUR_API_KEY_HERE"
  }
}
```

**⚠️ Security Note**: Never commit your API key to git. Add to `.gitignore`:
```
appsettings.Development.json
appsettings.*.json
```

### Step 3: Start the Application

**Terminal 1 - Backend:**
```bash
cd FirstReactApplication/FirstReactApplication.Server
dotnet run
```
✅ Backend ready at `https://localhost:7274`

**Terminal 2 - Frontend:**
```bash
cd FirstReactApplication/firstreactapplication.client
npm run dev
```
✅ Frontend ready at `http://localhost:5173`

### Step 4: Test the AI Agent

1. Open browser to `http://localhost:5173`
2. Search for a city (e.g., "London")
3. You'll see:
   - 🔮 **AI Weather Forecast** - AI-generated prediction
   - ⚠️ **Weather Alerts** - Any detected severe conditions
   - 💡 **Recommendations** - What to wear, activities to do

---

## Features in Detail

### 1. AI Weather Forecast 🔮

Claude AI analyzes current weather and provides a brief, actionable forecast:

```
"Expect continued warm conditions with no significant changes. 
Stay hydrated and seek shade during peak hours."
```

**What it considers:**
- Current temperature, humidity, wind speed
- Temperature ranges (min/max)
- Pressure changes
- Visibility conditions

**Confidence Score:**
- Shows prediction reliability (0-100%)
- Drops if visibility is poor or data is uncertain

### 2. Smart Alerts ⚠️

Automatically detects severe weather and generates alerts:

**Alert Types:**
- 🌪️ **Storm** - Thunder, severe conditions
- 🔥 **Heat** - Extreme temperatures (>35°C)
- ❄️ **Cold** - Freezing conditions (<0°C)
- 💨 **Wind** - High winds (>10 m/s)
- 💧 **Humidity** - Oppressive humidity + heat
- 👁️ **Visibility** - Low visibility conditions

**Severity Levels:**
- 🔴 **Critical** - Take immediate action
- 🟠 **High** - Be cautious
- 🟡 **Medium** - Be aware
- 🔵 **Low** - Minor concern

### 3. Recommendations 💡

Personalized advice based on current weather:

✅ What to wear (layers, sunscreen, etc.)  
✅ Outdoor activities to do or avoid  
✅ Health precautions  
✅ Travel safety tips  
✅ Hydration reminders  

---

## API Endpoints

All endpoints are prefixed with `/api/weather-prediction`

### Get Full Analysis
```
GET /api/weather-prediction/analyze?city=London
```

**Response:**
```json
{
  "city": "London",
  "country": "GB",
  "forecastText": "Generally pleasant weather...",
  "alerts": [
    {
      "type": "Wind",
      "severity": "Medium",
      "message": "Strong winds - 12.5 m/s",
      "recommendation": "Secure loose objects, use caution when driving"
    }
  ],
  "recommendations": [
    "Wear light, breathable clothing",
    "Use sunscreen SPF 30+",
    "Bring a light rain jacket"
  ],
  "confidenceScore": 87,
  "generatedAt": "2024-09-09T10:30:00Z",
  "nextUpdateTime": "2024-09-09T11:30:00Z"
}
```

### Get Only Alerts
```
GET /api/weather-prediction/alerts?city=London
```

### Get Only Recommendations
```
GET /api/weather-prediction/recommendations?city=London
```

---

## How Caching Works

**Smart Caching:**
- Predictions are cached for **1 hour**
- Same city searched within 1 hour returns cached result
- Saves API costs and improves performance
- Cache auto-clears after expiration

**Fallback Behavior:**
- If Claude API key missing → Shows demo predictions
- If Claude API fails → Returns demo predictions with logging
- Your app never breaks, even if AI fails

---

## Without API Key (Demo Mode)

The app works **without** an Anthropic API key!

**Demo Mode Features:**
- ✅ Shows realistic demo predictions
- ✅ Displays contextual alerts (based on temperature, humidity, wind)
- ✅ Provides practical recommendations
- ✅ Full UI/UX experience

**Limitations:**
- Predictions are rule-based, not AI-generated
- Good for testing UI without API costs
- Perfect for development/demo

**To Enable Demo Mode:**
Leave `Anthropic.ApiKey` empty in `appsettings.Development.json`

---

## Frontend Components

### WeatherPrediction.tsx
Main component that displays:
- Forecast card with confidence score
- Alert cards with color-coded severity
- Recommendations list
- Loading/error states

**Props:**
```typescript
interface WeatherPredictionProps {
  city: string;           // City to analyze
  onDataReceived?: (data) => void;  // Optional callback
}
```

**Usage in Weather.tsx:**
```tsx
<WeatherPrediction city={city} />
```

### Styling (WeatherPrediction.css)
- Beautiful gradient cards (purple, pink, cyan)
- Responsive design for mobile/tablet/desktop
- Smooth animations and transitions
- Color-coded alert severity indicators

---

## Backend Architecture

### IWeatherPredictionService
Interface defining prediction service:
```csharp
Task<WeatherPrediction> AnalyzeWeatherAsync(string city, WeatherDto weather);
Task<List<WeatherAlert>> GetAlertsAsync(string city, WeatherDto weather);
Task<List<string>> GetRecommendationsAsync(string city, WeatherDto weather);
```

### WeatherPredictionService
Implementation with:
- ✅ Claude API integration
- ✅ Error handling & fallbacks
- ✅ Demo mode for no API key
- ✅ Memory caching
- ✅ Confidence scoring
- ✅ Logging

### Models
- **WeatherPrediction** - Complete prediction data
- **WeatherAlert** - Alert with type, severity, message, recommendation

### WeatherPredictionController
REST API endpoints for:
- `/analyze` - Full analysis
- `/alerts` - Alerts only
- `/recommendations` - Recommendations only

---

## Configuration

### appsettings.Development.json
```json
{
  "Anthropic": {
    "ApiKey": "sk-ant-YOUR_KEY"
  }
}
```

### Environment Variables (Alternative)
```bash
export ANTHROPIC_API_KEY=sk-ant-YOUR_KEY
```

Then update `WeatherPredictionService.cs`:
```csharp
_claudeApiKey = configuration["Anthropic:ApiKey"] ?? 
                Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
```

---

## Customization

### Change Claude Model

In `WeatherPredictionService.cs`:
```csharp
private const string ClaudeModel = "claude-opus-5";  // Change this
```

**Available Models:**
- `claude-opus-5` - Most capable, slower, more expensive
- `claude-sonnet-5` - Balanced (recommended)
- `claude-haiku-4-5` - Fastest, cheapest, limited capability

### Adjust Cache Duration

In `WeatherPredictionService.cs`:
```csharp
_memoryCache.Set(cacheKey, prediction, TimeSpan.FromHours(2));  // Change duration
```

### Customize Prompts

Edit the prompt strings in `WeatherPredictionService.cs`:
- `GetAlertsAsync()` - Alert detection prompt
- `GetRecommendationsAsync()` - Recommendation prompt
- `GetForecastAnalysisAsync()` - Forecast prompt

---

## Troubleshooting

### Issue: "No predictions showing"
**Causes & Fixes:**
- API key not set → Add to `appsettings.Development.json`
- API key invalid → Double-check at https://console.anthropic.com
- Demo mode → Should show demo predictions anyway
- Check browser console for errors → F12 → Console tab

### Issue: "API calls failing"
**Causes & Fixes:**
- Check API key is correct: `sk-ant-...` format
- Check internet connection
- Check console logs: `dotnet run` terminal shows errors
- Verify API key has credits left

### Issue: "Predictions are generic/demo"
**Causes & Fixes:**
- API key not configured → Check `appsettings.Development.json`
- API key empty → Add your actual key
- Running in demo mode intentionally → This is normal!

### Issue: "Same prediction showing"
- This is **caching working correctly!**
- Predictions cache for 1 hour
- Search different city to see new prediction
- Or wait 1 hour for cache to expire

### Debug Mode

Add verbose logging in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "FirstReactApplication.Server": "Debug"
    }
  }
}
```

Then check `dotnet run` terminal for detailed logs.

---

## Cost Estimation

**Claude API Pricing** (as of 2024):
- Input tokens: $3 per 1M tokens
- Output tokens: $15 per 1M tokens

**Per-Request Estimate:**
- Input: ~300 tokens = $0.0009
- Output: ~150 tokens = $0.00225
- **Total: ~$0.003 per request**

**Monthly Estimates:**
- 100 requests/day = $0.90/month
- 1000 requests/day = $9/month
- 10000 requests/day = $90/month

Free trial ($5) covers ~1,600 requests before paid.

---

## Security Best Practices

✅ **DO:**
- Store API key in `appsettings.Development.json` (don't commit)
- Use environment variables for production
- Add `.gitignore` entries for config files
- Rotate API keys regularly
- Monitor usage on Anthropic console

❌ **DON'T:**
- Commit API keys to git
- Expose API keys in frontend code
- Use API key in client-side requests
- Share API keys in public channels
- Hard-code API keys in source

---

## Next Steps

### Immediate
1. ✅ Get Anthropic API key
2. ✅ Add to `appsettings.Development.json`
3. ✅ Run backend and frontend
4. ✅ Test with different cities

### Short Term
- [ ] Add to production appsettings
- [ ] Test with various weather conditions
- [ ] Customize prompts for your use case
- [ ] Optimize Claude model choice

### Long Term
- [ ] Add historical prediction tracking
- [ ] Implement user feedback loop
- [ ] Train custom Claude model
- [ ] Add email/push notifications
- [ ] Build admin dashboard for analytics

---

## Resources

📚 **Documentation:**
- [Anthropic API Docs](https://docs.anthropic.com)
- [Claude Models Guide](https://docs.anthropic.com/claude/reference/getting-started-with-the-api)
- [OpenWeatherMap API](https://openweathermap.org/api)
- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [React Docs](https://react.dev)

🎓 **Learn More:**
- Claude Prompt Engineering: https://docs.anthropic.com/en/docs/build-a-chatbot-with-claude
- Weather API Integration: https://openweathermap.org/guide
- ASP.NET Memory Cache: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory

💬 **Support:**
- Anthropic Support: https://support.anthropic.com
- Stack Overflow: Tag with `claude-api`

---

## Architecture Diagram

```
┌─────────────────────────────────────┐
│        React Frontend               │
│  ┌──────────────────────────────┐   │
│  │  Weather.tsx                 │   │
│  │  ├─ Current Weather          │   │
│  │  └─ WeatherPrediction.tsx    │   │
│  └──────────────────────────────┘   │
└────────────┬────────────────────────┘
             │ HTTP GET /api/weather-prediction/analyze?city=London
             ↓
┌─────────────────────────────────────────────────────────┐
│        .NET Core Backend (localhost:7274)               │
│  ┌──────────────────────────────────────────────────┐   │
│  │  WeatherPredictionController                     │   │
│  │  ├─ /analyze → AnalyzeWeather()                 │   │
│  │  ├─ /alerts → GetAlerts()                       │   │
│  │  └─ /recommendations → GetRecommendations()     │   │
│  └──────────────────────────────────────────────────┘   │
│                        ↓                                  │
│  ┌──────────────────────────────────────────────────┐   │
│  │  IWeatherPredictionService                       │   │
│  └──────────────────────────────────────────────────┘   │
│                        ↓                                  │
│  ┌──────────────────────────────────────────────────┐   │
│  │  WeatherPredictionService                        │   │
│  │  ├─ GetCurrentWeather() → OpenWeatherMapService │   │
│  │  ├─ CallClaudeAPIAsync()                        │   │
│  │  └─ CacheResults() → IMemoryCache               │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────┬─────────────────────────────────────┘
                     │ HTTP POST to Claude API
                     ↓
        ┌────────────────────────────┐
        │  Anthropic Claude API      │
        │  ├─ Model: claude-3-5-...  │
        │  ├─ Input: Weather Data    │
        │  └─ Output: Prediction     │
        └────────────────────────────┘
```

---

## Version Info

- **Framework:** .NET Core 8
- **Frontend:** React 18+ with TypeScript
- **AI Model:** Claude 3.5 Sonnet
- **Cache:** ASP.NET Core In-Memory Cache
- **Weather API:** OpenWeatherMap v2.5

---

## License & Attribution

This Weather Prediction Agent uses:
- 🌦️ **OpenWeatherMap** - Weather data
- 🤖 **Anthropic Claude** - AI predictions
- ⚛️ **React** - Frontend framework
- 🟦 **.NET Core** - Backend framework

Always credit OpenWeatherMap in your UI as the data source.

---

## Support & Feedback

Found a bug or have a feature request?
1. Check existing issues on GitHub
2. Create detailed bug report with:
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots/logs
   - OS and browser version

---

Happy predicting! 🌤️🤖
