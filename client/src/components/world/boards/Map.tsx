import { useContext } from 'react';
import Board from '../../../types/board';
import Region from './Region';
import regions from '../../../data/regions';
import OrderEntryContext from '../../context/OrderEntryContext';
import UnitType from '../../../types/enums/unitType';
import InputMode from '../../../types/enums/inputMode';
import { OrderType } from '../../../types/order';
import WorldContext from '../../context/WorldContext';
import { findUnit } from '../../../types/world';
import Phase from '../../../types/enums/phase';

type MapProps = {
  board: Board;
  isShowingCoasts?: boolean;
};

const Map = ({ board, isShowingCoasts = false }: MapProps) => {
  const { world } = useContext(WorldContext);
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
            />
          );
        }

        const hasFleet = board.units[region]?.type === UnitType.Fleet;

        const showCoast = {
          [InputMode.None]: hasFleet || board.phase === Phase.Winter,
          [InputMode.Hold]: hasFleet,
          [InputMode.Move]: currentOrder?.unit?.type === UnitType.Fleet,
          [InputMode.Support]:
            currentOrder?.$type === OrderType.Support &&
            ((!currentOrder.supportLocation && hasFleet) ||
              (currentOrder.supportLocation !== null &&
                findUnit(world, currentOrder.supportLocation)?.type === UnitType.Fleet)),
          [InputMode.Convoy]: hasFleet,
          [InputMode.Build]: true,
          [InputMode.Disband]: hasFleet,
        }[currentMode];

        return (
          <Region
            key={region}
            id={region}
            timeline={board.timeline}
            year={board.year}
            phase={board.phase}
            owner={board.centres[baseRegion]}
            unit={board.units[region]}
            isVisible={showCoast || isShowingCoasts}
          />
        );
      })}
    </div>
  );
};

export default Map;
