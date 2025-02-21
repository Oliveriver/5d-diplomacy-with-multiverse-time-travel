import { useContext, useEffect, useState } from 'react';
import GameContext from './context/GameContext';
import SetupRoot from './SetupRoot';
import { WorldContextProvider } from './context/WorldContext';
import GameRoot from './GameRoot';
import colours, { coloursVariables, lightColours, darkColours } from '../utils/colours';
import { Colours } from '../utils/types';

const AppRoot = () => {
  const { game } = useContext(GameContext);

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

  const cssVariables = Object.entries(coloursVariables).reduce((acc, [key, value]) => {
    const colour = isDarkMode ? darkColours[key as keyof Colours] : lightColours[key as keyof Colours];
    return { ...acc, [value]: colour };
  }, {});

  return (
    <div
      style={{
        ...cssVariables,
        color: colours.uiForeground,
        backgroundColor: colours.uiPageBackground,
      } as React.CSSProperties}
    >
      {!game && <SetupRoot />}
      {game && (
        <WorldContextProvider>
          <GameRoot />
        </WorldContextProvider>
      )}
    </div>
  );
};

export default AppRoot;
