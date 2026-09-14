import { StrictMode, useState } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import Weather from './Weather.tsx'

//const App = () => {
//  const [currentPage, setCurrentPage] = useState<'home' | 'weather'>('weather');

//  return (
//    <div>
//      <nav style={{
//        padding: '15px',
//        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
//        display: 'flex',
//        gap: '20px',
//        justifyContent: 'center',
//        boxShadow: '0 2px 8px rgba(0,0,0,0.1)'
//      }}>
//        <button
//          onClick={() => setCurrentPage('weather')}
//          style={{
//            padding: '10px 20px',
//            border: 'none',
//            background: currentPage === 'weather' ? 'white' : 'rgba(255,255,255,0.3)',
//            color: currentPage === 'weather' ? '#667eea' : 'white',
//            borderRadius: '8px',
//            cursor: 'pointer',
//            fontWeight: 'bold',
//            fontSize: '1em',
//            transition: 'all 0.3s ease'
//          }}
//        >
//          🌤️ Weather App
//        </button>
//        <button
//          onClick={() => setCurrentPage('home')}
//          style={{
//            padding: '10px 20px',
//            border: 'none',
//            background: currentPage === 'home' ? 'white' : 'rgba(255,255,255,0.3)',
//            color: currentPage === 'home' ? '#667eea' : 'white',
//            borderRadius: '8px',
//            cursor: 'pointer',
//            fontWeight: 'bold',
//            fontSize: '1em',
//            transition: 'all 0.3s ease'
//          }}
//        >
//          🏠 Home
//        </button>
//      </nav>
//      {currentPage === 'weather' && <Weather />}
//    </div>
//  );
//};

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Weather />
  </StrictMode>,
)
