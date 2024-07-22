import { useContext } from 'react';
import InputMode from '../../types/enums/inputMode';
import Button from './common/Button';
import OrderEntryContext from '../context/OrderEntryContext';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';

type InputModeButtonProps = {
  mode: InputMode;
};

const InputModeButton = ({ mode }: InputModeButtonProps) => {
  const { currentMode, availableModes, dispatch } = useContext(OrderEntryContext);

  return (
    <Button
      text={mode}
      onClick={() => dispatch({ $type: OrderEntryActionType.SetMode, mode })}
      isSelected={mode === currentMode}
      isDisabled={!availableModes.includes(mode)}
      minWidth={128}
    />
  );
};

export default InputModeButton;
