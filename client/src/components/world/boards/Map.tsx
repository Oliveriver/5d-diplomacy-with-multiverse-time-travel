import { useContext } from 'react';
import Board from '../../../types/board';
import Region from './Region';
import regions from '../../../data/regions';
import OrderEntryContext from '../../context/OrderEntryContext';
import UnitType from '../../../types/enums/unitType';
import InputMode from '../../../types/enums/inputMode';

type MapProps = {
  board: Board;
  isActive: boolean;
};

const Map = ({ board, isActive }: MapProps) => {
  const { currentMode, currentOrder } = useContext(OrderEntryContext);

  return (
    <div className="mt-1">
      {Object.keys(regions).map((region) => {
        const baseRegion = region.split('_')[0];
        const isCoast = region !== baseRegion;

        if (!isCoast) {
          return (
            <Region
              key={region}
              id={region}
              timeline={board.timeline}
              year={board.year}
              phase={board.phase}
              owner={board.centres[region]}
              unit={board.units[region]}
              isActive={isActive}
            />
          );
        }

        // TODO fix for fleet supporting army on space with coasts and army supporting fleet to coast
        const showCoast =
          currentOrder?.unit?.type !== UnitType.Army &&
          ((currentOrder?.unit?.type === UnitType.Fleet && currentMode !== InputMode.Convoy) ||
            board.units[region] !== undefined ||
            currentMode === InputMode.Build);

        return (
          <Region
            key={region}
            id={region}
            timeline={board.timeline}
            year={board.year}
            phase={board.phase}
            owner={board.centres[baseRegion]}
            unit={board.units[region]}
            isActive={isActive}
            isVisible={showCoast}
          />
        );
      })}
    </div>
  );
};

export default Map;
