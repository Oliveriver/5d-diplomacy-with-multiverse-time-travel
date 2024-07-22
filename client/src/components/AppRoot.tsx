import { useContext } from 'react';
import GameContext from './context/GameContext';
import SetupRoot from './SetupRoot';
import { WorldContextProvider } from './context/WorldContext';
import GameRoot from './GameRoot';

const AppRoot = () => {
  const { game } = useContext(GameContext);

  return (
    <>
      {!game && <SetupRoot />}
      {game && (
        <WorldContextProvider>
          <GameRoot />
        </WorldContextProvider>
      )}
    </>
  );
};

export default AppRoot;
