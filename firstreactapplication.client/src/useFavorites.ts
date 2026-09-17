import { useState, useEffect } from 'react';

export const useFavorites = (initialCity: string = '') => {
  const [favorites, setFavorites] = useState<string[]>(() => {
    const saved = localStorage.getItem('favorite-cities');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch {
        return [initialCity];
      }
    }
    return [initialCity];
  });

  useEffect(() => {
    localStorage.setItem('favorite-cities', JSON.stringify(favorites));
  }, [favorites]);

  const addFavorite = (city: string) => {
    setFavorites(prev => {
      if (prev.includes(city)) return prev;
      return [...prev, city];
    });
  };

  const removeFavorite = (city: string) => {
    setFavorites(prev => prev.filter(c => c !== city));
  };

  const isFavorite = (city: string) => favorites.includes(city);

  return { favorites, addFavorite, removeFavorite, isFavorite };
};
