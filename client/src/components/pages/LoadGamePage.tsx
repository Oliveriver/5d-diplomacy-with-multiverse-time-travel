import { useContext, useState } from 'react';
import Nation from '../../types/enums/nation';
import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../user-interface/common/Button';
import NationSelect from '../user-interface/NationSelect';
import GameContext from '../context/GameContext';
import Error from '../user-interface/common/Error';
import Select from '../user-interface/common/Select';

type LoadGamePageProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const LoadGamePage = ({ setViewOption }: LoadGamePageProps) => {
  const { loadGame, exitGame, isLoading, error } = useContext(GameContext);

  const [isSandbox, setIsSandbox] = useState(false);
  const [player, setPlayer] = useState<Nation>();
  const [file, setFile] = useState<File | null>(null);

  const onLoadPressed = () => loadGame(isSandbox, player ?? null, file!);

  const onFileChanged = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setFile(e.target.files[0]);
    }
  };

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
      <input className="w-64 pr-4" type="file" accept="application/json" onChange={onFileChanged} />
      <Button
        text="Load"
        minWidth={256}
        onClick={onLoadPressed}
        isBusy={isLoading}
        isDisabled={file === null}
      />
      {error && <Error error={error} />}
      <div className="absolute bottom-10">
        <Button text="Back" minWidth={96} onClick={onBackPressed} />
      </div>
    </div>
  );
};

export default LoadGamePage;
