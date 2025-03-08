import { useContext } from 'react';
import useSetAvailableInputModes from '../../hooks/useSetAvailableInputModes';
import InputMode from '../../types/enums/inputMode';
import InputModeButton from './InputModeButton';
import { rasteriseEnabled, rasteriseScaleThreshold } from '../../utils/constants';
import ScaleContext from '../context/ScaleContext';

const InputModeButtonList = () => {
  useSetAvailableInputModes();

  const inputButtons = Object.values(InputMode)
    .filter((mode) => mode !== InputMode.None)
    .map((mode) => <InputModeButton mode={mode} key={mode} />);

  const { scale } = useContext(ScaleContext);

  const showRasterised = rasteriseEnabled && scale < rasteriseScaleThreshold;

  if (showRasterised) {
    return null;
  }

  return (
    <div className="absolute bottom-10 w-screen flex gap-10 justify-center pointer-events-none">
      {inputButtons}
    </div>
  );
};

export default InputModeButtonList;
