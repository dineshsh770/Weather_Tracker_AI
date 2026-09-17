import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import Weather from './Weather.tsx'
import { ThemeProvider } from './ThemeContext.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider>
      <Weather />
    </ThemeProvider>
  </StrictMode>,
)
