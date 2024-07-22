import { useContext, useState } from 'react';
import SetupViewOption from '../../types/enums/setupViewOption';
import Nation from '../../types/enums/nation';
import Button from '../interface/common/Button';
import NationSelect from '../interface/NationSelect';
import GameContext from '../context/GameContext';
import Error from '../interface/common/Error';

type JoinGameProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const JoinGamePage = ({ setViewOption }: JoinGameProps) => {
  const { joinGame, exitGame, isLoading, error } = useContext(GameContext);

  const [gameId, setGameId] = useState<number>();
  const [player, setPlayer] = useState<Nation>();

  const onGameIdChanged = (value: string) => {
    const parsedValue = parseInt(value, 10);
    if (
      parsedValue.toString() !== value ||
      Number.isNaN(parsedValue) ||
      !Number.isInteger(parsedValue)
    ) {
      setGameId(undefined);
      return;
    }
    setGameId(parsedValue);
  };

  const onJoinPressed = () => {
    if (gameId === undefined) return;
    joinGame(gameId ?? 0, player ?? null);
  };

  const onBackPressed = () => {
    exitGame();
    setViewOption(SetupViewOption.None);
  };

  return (
    <div className="flex flex-col w-screen h-screen items-center gap-4 pt-24">
      <img alt="Logo" src="./logo.png" className="w-64 pb-8" />
      <input
        type="text"
        className="w-64 h-16 p-4 border-4 rounded-xl text-lg"
        placeholder="Game ID"
        onChange={(event) => onGameIdChanged(event.target.value)}
      />
      <NationSelect selectedNation={player} setSelectedNation={setPlayer} />
      <Button
        text="Join"
        minWidth={256}
        onClick={onJoinPressed}
        isDisabled={gameId === undefined}
        isBusy={isLoading}
      />
      {error && <Error error={error} />}
      <div className="absolute bottom-10">
        <Button text="Back" minWidth={96} onClick={onBackPressed} />
      </div>
    </div>
  );
};

export default JoinGamePage;
