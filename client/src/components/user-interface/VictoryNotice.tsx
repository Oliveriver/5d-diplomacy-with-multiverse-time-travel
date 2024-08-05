import { useContext } from 'react';
import { getNationColour } from '../../types/enums/nation';
import WorldContext from '../context/WorldContext';

const VictoryNotice = () => {
  const { world } = useContext(WorldContext);
  const winner = world?.winner;
  if (!winner) return null;

  return (
    <div className="absolute top-10 w-screen flex justify-center pointer-events-none">
      <p className="text-6xl font-bold" style={{ color: getNationColour(winner) }}>
        {`${winner} wins!`}
      </p>
    </div>
  );
};

export default VictoryNotice;
