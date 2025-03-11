import { useContext, useState } from 'react';
import Nation from '../../types/enums/nation';
import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../user-interface/common/Button';
import NationSelect from '../user-interface/NationSelect';
import GameContext from '../context/GameContext';
import Error from '../user-interface/common/Error';
import Select from '../user-interface/common/Select';

type NewGamePageProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const NewGamePage = ({ setViewOption }: NewGamePageProps) => {
  const { createGame, exitGame, isLoading, error } = useContext(GameContext);

  const [isSandbox, setIsSandbox] = useState(false);
  const [player, setPlayer] = useState<Nation>();
  const [hasStrictAdjacencies, setHasStrictAdjacencies] = useState(true);

  const onCreatePressed = () => createGame(isSandbox, player ?? null, hasStrictAdjacencies);

  const onBackPressed = () => {
    exitGame();
    setViewOption(SetupViewOption.None);
  };

  return (
    <div className="flex flex-col w-screen h-screen items-center gap-4 pt-24">
      <img alt="Logo" src="./logo.png" className="w-64 h-64 mb-8" />
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
      <Select
        options={[
          {
            value: 'true',
            text: 'Strict adjacencies',
            hint: 'Multiverse moves must arrive at the equivalent region to the one they left',
          },
          {
            value: 'false',
            text: 'Loose adjacencies',
            hint: 'Multiverse moves can arrive at the equivalent region to the one they left or any adjacent region in that board',
          },
        ]}
        setValue={(value) => setHasStrictAdjacencies(value === 'true')}
        selectedValue={hasStrictAdjacencies ? 'true' : 'false'}
      />
      <Button text="Create" minWidth={256} onClick={onCreatePressed} isBusy={isLoading} />
      {error && <Error error={error} />}
      <div className="absolute bottom-10">
        <Button text="Back" minWidth={96} onClick={onBackPressed} />
      </div>
    </div>
  );
};

export default NewGamePage;
