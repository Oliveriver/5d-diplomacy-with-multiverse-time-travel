import { useContext, useState } from 'react';
import SetupViewOption from '../../types/enums/setupViewOption';
import Nation from '../../types/enums/nation';
import Button from '../user-interface/common/Button';
import NationSelect from '../user-interface/NationSelect';
import GameContext from '../context/GameContext';
import Error from '../user-interface/common/Error';
import TextInput from '../user-interface/common/TextInput';
import { isInteger } from '../../utils/numberUtils';
import Select from '../user-interface/common/Select';

type JoinGameProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const JoinGamePage = ({ setViewOption }: JoinGameProps) => {
  const { joinGame, exitGame, isLoading, error } = useContext(GameContext);

  const [gameId, setGameId] = useState<number>();
  const [isSandbox, setIsSandbox] = useState(false);
  const [player, setPlayer] = useState<Nation>();

  const onGameIdChanged = (value: string) => {
    if (!isInteger(value)) {
      setGameId(undefined);
      return;
    }

    const parsedValue = parseInt(value, 10);
    setGameId(parsedValue);
  };

  const onJoinPressed = () => {
    if (gameId === undefined) return;
    joinGame(gameId ?? 0, isSandbox, player ?? null);
  };

  const onBackPressed = () => {
    exitGame();
    setViewOption(SetupViewOption.None);
  };

  return (
    <div className="flex flex-col w-screen h-screen items-center gap-4 pt-24">
      <img alt="Logo" src="./logo.png" className="w-64 h-64 mb-8" />
      <TextInput placeholder="Game ID" onChange={onGameIdChanged} />
      <Select
        options={[
          {
            value: 'false',
            text: 'Normal',
          },
          {
            value: 'true',
            text: 'Sandbox',
          },
        ]}
        setValue={(value) => setIsSandbox(value === 'true')}
        selectedValue={isSandbox ? 'true' : 'false'}
      />
      {!isSandbox && <NationSelect selectedNation={player} setSelectedNation={setPlayer} />}
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
