import { QueryClientProvider } from '@tanstack/react-query';
import queryClient from './api/queryClient';
import { GameContextProvider } from './components/context/GameContext';
import AppRoot from './components/AppRoot';
import { DarkModeContextContextProvider } from './components/context/DarkModeContext';

const App = () => (
  <QueryClientProvider client={queryClient}>
    <GameContextProvider>
      <DarkModeContextContextProvider>
        <AppRoot />
      </DarkModeContextContextProvider>
    </GameContextProvider>
  </QueryClientProvider>
);

export default App;
