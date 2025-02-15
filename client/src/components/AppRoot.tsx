import { useContext } from 'react';
import GameContext from './context/GameContext';
import SetupRoot from './SetupRoot';
import { WorldContextProvider } from './context/WorldContext';
import GameRoot from './GameRoot';
import colours from '../utils/colours';

const AppRoot = () => {
  const { game } = useContext(GameContext);

  return (
    <div
      style={{
        color: colours.uiTextColour,
        backgroundColor: colours.uiPageBackground,
      }}
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
