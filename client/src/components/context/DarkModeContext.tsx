import { PropsWithChildren, createContext, useEffect, useState } from 'react';

const DarkModeContext = createContext(false);

export const DarkModeContextContextProvider = ({ children }: PropsWithChildren) => {
  const [isDarkMode, setIsDarkMode] = useState(false);
  useEffect(() => {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    if (mediaQuery.matches) {
      setIsDarkMode(true);
    }

    // Watch for changes to the user's preferred colour scheme
    const listener = () => setIsDarkMode(mediaQuery.matches);
    mediaQuery.addEventListener('change', listener);
    return () => mediaQuery.removeEventListener('change', listener);
  }, []);

  return <DarkModeContext.Provider value={isDarkMode}>{children}</DarkModeContext.Provider>;
};

export default DarkModeContext;
