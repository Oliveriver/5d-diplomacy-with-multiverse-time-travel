import { useContext } from 'react';
import Button from './common/Button';
import GameContext from '../context/GameContext';

const ExitButton = () => {
  const { exitGame } = useContext(GameContext);

  return (
    <div className="absolute left-10 bottom-10">
      <Button text="Exit" minWidth={96} onClick={exitGame} />
    </div>
  );
};

export default ExitButton;
