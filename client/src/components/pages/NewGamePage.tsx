import { useContext, useState } from 'react';
import Nation from '../../types/enums/nation';
import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../interface/common/Button';
import NationSelect from '../interface/NationSelect';
import GameContext from '../context/GameContext';
import Error from '../interface/common/Error';
import Select from '../interface/common/Select';

type NewGamePageProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const NewGamePage = ({ setViewOption }: NewGamePageProps) => {
  const { createGame, exitGame, isLoading, error } = useContext(GameContext);

  const [isSandbox, setIsSandbox] = useState(false);
  const [player, setPlayer] = useState<Nation>();

  const onCreatePressed = () => createGame(isSandbox, player ?? null);

  const onBackPressed = () => {
    exitGame();
    setViewOption(SetupViewOption.None);
  };

  return (
    <div className="flex flex-col w-screen h-screen items-center gap-4 pt-24">
      <img alt="Logo" src="./logo.png" className="w-64 pb-8" />
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
      <Button text="Create" minWidth={256} onClick={onCreatePressed} isBusy={isLoading} />
      {error && <Error error={error} />}
      <div className="absolute bottom-10">
        <Button text="Back" minWidth={96} onClick={onBackPressed} />
      </div>
    </div>
  );
};

export default NewGamePage;
