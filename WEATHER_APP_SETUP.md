# Weather App - Setup & Documentation

## Overview
This is a full-stack weather application built with:
- **Backend**: .NET Core 8 (C#)
- **Frontend**: React with TypeScript
- **API**: Free OpenWeatherMap API for real-time weather data

## Project Structure

```
FirstReactApplication/
├── FirstReactApplication.Server/          # .NET Core Backend
│   ├── Controllers/
│   │   └── WeatherController.cs           # Weather API endpoints
│   ├── Services/
│   │   ├── IWeatherService.cs             # Service interface
│   │   └── OpenWeatherMapService.cs       # Weather API integration
│   ├── Models/
│   │   ├── WeatherResponse.cs             # API response models
│   │   └── WeatherDto.cs                  # Data transfer object
│   └── Program.cs                         # Application configuration
│
└── firstreactapplication.client/          # React Frontend
    └── src/
        ├── Weather.tsx                    # Weather component
        ├── Weather.css                    # Weather styles
        ├── Home.tsx                       # Home page
        └── main.tsx                       # App entry & routing
```

## Features

✅ **Search by City Name** - Enter any city to get real-time weather
✅ **Geolocation Support** - Use your device location for instant weather
✅ **Real-time Data** - Live updates from OpenWeatherMap API
✅ **Comprehensive Weather Info**:
   - Current temperature (feels like)
   - Min/Max temperatures
   - Humidity and wind speed
   - Pressure and visibility
   - Sunrise/sunset times
   - Weather description with icons

✅ **Responsive Design** - Works on desktop and mobile
✅ **Error Handling** - Graceful error messages
✅ **Beautiful UI** - Modern gradient design with smooth animations

## Setup Instructions

### Prerequisites
- .NET Core 8 SDK
- Node.js (v16+)
- npm or yarn

### Backend Setup

1. **Navigate to backend directory**:
   ```bash
   cd FirstReactApplication.Server
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

### Frontend Setup

1. **Navigate to frontend directory**:
   ```bash
   cd firstreactapplication.client
   ```

2. **Install npm dependencies**:
   ```bash
   npm install
   ```

## Running the Application

### Option 1: Using Visual Studio
1. Open `FirstReactApplication.sln` in Visual Studio
2. Set `FirstReactApplication.Server` as the startup project
3. Press F5 to run
4. The app will start at `https://localhost:7274` (or similar)
5. React dev server will run on `https://localhost:60826`

### Option 2: Command Line

**Terminal 1 - Backend**:
```bash
cd FirstReactApplication.Server
dotnet run
```

**Terminal 2 - Frontend**:
```bash
cd firstreactapplication.client
npm run dev
```

Then navigate to `http://localhost:5173` (or the URL shown by Vite)

## API Endpoints

### Get Weather by City
```
GET /api/weather/city?city=London
```

**Response**:
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

### Get Weather by Coordinates
```
GET /api/weather/coordinates?latitude=51.5074&longitude=-0.1278
```

## Weather Icons
Icons are fetched from OpenWeatherMap's icon CDN and update dynamically based on weather conditions:
- ☀️ Clear sky
- ⛅ Partly cloudy
- ☁️ Cloudy
- 🌧️ Rainy
- ⛈️ Thunderstorm
- And more...

## Configuration

### API Key
The service uses a default free OpenWeatherMap API key. For production:

1. Get your free API key from [OpenWeatherMap](https://openweathermap.org/api)
2. Update in `appsettings.json`:
   ```json
   {
     "OpenWeatherMap": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```
3. Or set environment variable: `OpenWeatherMap__ApiKey=your-api-key-here`

## Free API Tier Limits
- OpenWeatherMap Free tier:
  - Current weather: 1,000 calls/day
  - 5-day forecast: 1,000 calls/day
  - Perfect for development and small projects

## Troubleshooting

### CORS Errors
The backend is configured to allow all origins. If you see CORS errors:
- Ensure the backend is running
- Check that the API URL matches (Protocol, Host, Port)

### API Rate Limiting
If you get "429 Too Many Requests":
- Check API call frequency
- Consider upgrading OpenWeatherMap subscription
- Implement request caching

### Geolocation Not Working
- Browser must be HTTPS (or localhost)
- User must grant permission when prompted
- Check browser privacy settings

## Performance Tips

1. **Cache Results** - Store weather data locally to reduce API calls
2. **Debounce Searches** - Add delay on search input to prevent spam
3. **Lazy Load Icons** - Images load after main content
4. **Error Boundaries** - Component error handling in place

## Production Deployment

### Build Frontend
```bash
cd firstreactapplication.client
npm run build
```

### Publish Backend
```bash
cd FirstReactApplication.Server
dotnet publish -c Release -o ./publish
```

### Deploy
- Backend: Deploy to Azure, AWS, or any .NET Core hosting
- Frontend: Already built into `wwwroot` by the SPA proxy

## Technologies Used

- **React 18+** - UI framework
- **TypeScript** - Type-safe JavaScript
- **CSS3** - Modern styling with gradients & animations
- **ASP.NET Core 8** - High-performance backend
- **HttpClient** - HTTP communication
- **OpenWeatherMap API** - Real-time weather data

## Contributing
Feel free to enhance the app with:
- 5-day forecast
- Multiple location tracking
- Weather alerts
- Dark mode toggle
- Unit testing

## License
Free to use and modify

## Support
For issues with the app, check:
1. Backend logs in console
2. Browser developer tools (F12)
3. Network requests in Network tab
4. OpenWeatherMap API status page
