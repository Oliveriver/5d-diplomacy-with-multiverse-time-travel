import { QueryClientProvider } from '@tanstack/react-query';
import queryClient from './api/queryClient';
import { GameContextProvider } from './components/context/GameContext';
import AppRoot from './components/AppRoot';

const App = () => (
  <QueryClientProvider client={queryClient}>
    <GameContextProvider>
      <AppRoot />
    </GameContextProvider>
  </QueryClientProvider>
);

export default App;
