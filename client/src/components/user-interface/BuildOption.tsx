import { useContext } from 'react';
import Nation from '../../types/enums/nation';
import UnitType from '../../types/enums/unitType';
import UnitIcon from '../world/boards/UnitIcon';
import Button from './common/Button';
import OrderEntryContext from '../context/OrderEntryContext';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';
import Location, { compareLocations } from '../../types/location';
import { OrderType } from '../../types/order';
import regions from '../../data/regions';
import RegionType from '../../types/enums/regionType';

type BuildOptionsProps = {
  location: Location;
  owner?: Nation;
  scaleFactor: number;
};

const BuildOptions = ({ location, owner, scaleFactor }: BuildOptionsProps) => {
  const { dispatch, currentOrder } = useContext(OrderEntryContext);

  if (
    !owner ||
    currentOrder?.$type !== OrderType.Build ||
    !compareLocations(currentOrder?.location, location) ||
    owner !== regions[location.region].homeNation
  ) {
    return null;
  }

  const select = (type: UnitType) =>
    dispatch({
      $type: OrderEntryActionType.Add,
      unit: { owner, type, mustRetreat: false },
      location,
    });

  if (regions[location.region].type !== RegionType.Coast) {
    return (
      <div className="absolute z-20 flex gap-2 -mx-4 -my-4 pointer-events-none">
        <Button minWidth={32} minHeight={32} onClick={() => select(UnitType.Army)}>
          <UnitIcon
            unit={{ owner, type: UnitType.Army, mustRetreat: false }}
            scaleFactor={scaleFactor}
          />
        </Button>
      </div>
    );
  }

  return (
    <div className="absolute z-20 flex gap-2 -mx-9 -my-4 pointer-events-none">
      <Button minWidth={32} minHeight={32} onClick={() => select(UnitType.Army)}>
        <UnitIcon
          unit={{ owner, type: UnitType.Army, mustRetreat: false }}
          scaleFactor={scaleFactor}
        />
      </Button>
      <Button minWidth={32} minHeight={32} onClick={() => select(UnitType.Fleet)}>
        <UnitIcon
          unit={{ owner, type: UnitType.Fleet, mustRetreat: false }}
          scaleFactor={scaleFactor}
        />
      </Button>
    </div>
  );
};

export default BuildOptions;
