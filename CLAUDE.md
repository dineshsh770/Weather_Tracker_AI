# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a React + TypeScript weather application built with Vite. The project has a client-side frontend (`firstreactapplication.client`) that displays real-time weather data and AI-powered weather predictions.

### Core Architecture

- **Client**: React 19 SPA with TypeScript, using Vite as the build tool
- **API Integration**: Proxied to a backend ASP.NET server (default: `http://localhost:5070`)
- **State Management**: React hooks (useState, useEffect) — no external state library
- **Styling**: CSS Modules (Weather.css, WeatherPrediction.css, index.css)
- **Linting**: Oxlint (no TypeScript type-aware linting enabled yet)

### Key Components

- **Weather.tsx**: Main component fetching current weather by city or geolocation
  - Handles API calls to `/api/weather/city` and `/api/weather/coordinates`
  - Manages search form, geolocation, and error states
  - Embeds `<WeatherPrediction>` as a child component
  
- **WeatherPrediction.tsx**: AI-powered weather forecasting component
  - Calls `/api/weather-prediction/analyze` endpoint
  - Displays alerts with severity levels, recommendations, and confidence scores
  - Color-coded severity (critical, high, medium, low)

### Proxy Routing

Vite dev server proxies:
- `^/api/*` → backend server
- `^/weatherforecast/*` → backend server
- HTTPS dev server uses certificates from `$APPDATA/ASP.NET/https` (Windows) or `~/.aspnet/https` (Unix)

## Common Commands

All commands run from `firstreactapplication.client/`:

```bash
# Development server with HMR
npm run dev

# Build for production (TypeScript check + Vite build)
npm run build

# Run Oxlint
npm run lint

# Preview production build locally
npm run preview
```

**Dev server details:**
- Default port: 60826 (configurable via `DEV_SERVER_PORT` env var)
- HTTPS enabled with self-signed dev certificates
- Hot Module Replacement (HMR) enabled

## Development Notes

### TypeScript Configuration

- Separate configs for app (`tsconfig.app.json`) and Vite/build (`tsconfig.node.json`)
- Main config (`tsconfig.json`) references both via "references"
- Path alias: `@/` → `./src/`

### Building

`npm run build` runs TypeScript type-checking first (`tsc -b`), then Vite bundling. If TypeScript fails, the build stops.

### Linting

Currently using basic Oxlint rules. To enable type-aware rules for production:
- Install `oxlint-tsgolint`
- Set `"typeAware": true` in `.oxlintrc.json`
- Add recommended React/TypeScript rules per README.md

### React Compiler

Not enabled (disabled for dev/build performance). See README.md for setup if needed.

## Backend API Contract

The frontend expects these endpoints on the proxied backend:

1. **GET `/api/weather/city?city={cityName}`**
   - Returns: `WeatherData` (temperature, humidity, wind, sunrise/sunset, etc.)
   - Used by Weather component for city-based searches

2. **GET `/api/weather/coordinates?latitude={lat}&longitude={lng}`**
   - Returns: `WeatherData`
   - Used for geolocation-based weather fetch

3. **GET `/api/weather-prediction/analyze?city={cityName}`**
   - Returns: `WeatherPredictionData` (forecast text, alerts, recommendations, confidence score)
   - Used by WeatherPrediction component for AI forecasts

If backend is unreachable or returns non-OK status, components show error messages. Network errors are caught and displayed to the user.

## Testing & Debugging

- No unit tests currently configured
- Check browser console and Vite dev server logs for client-side errors
- Use browser DevTools to inspect API responses from the backend
- Geolocation requires HTTPS (or localhost) for browser to allow access
