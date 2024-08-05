import { useContext } from 'react';
import Location from '../../../types/location';
import Unit from '../../../types/unit';
import WorldContext from '../../context/WorldContext';
import OrderPath from '../../world/orders/OrderPath';
import UnitIcon from '../../world/boards/UnitIcon';
import { compareBoards, getBoardName } from '../../../types/board';
import { findUnit } from '../../../types/world';

type ConvoyLabelProps = {
  unit: Unit | null;
  location: Location;
  convoyLocation?: Location | null;
  destination?: Location | null;
};

const ConvoyLabel = ({ unit, location, convoyLocation, destination }: ConvoyLabelProps) => {
  const { world } = useContext(WorldContext);

  if (!unit || !convoyLocation || !destination) return null;

  const convoyedUnit = findUnit(world, convoyLocation);

  const convoyLocationLabel = !compareBoards(location, convoyLocation)
    ? `(${getBoardName(convoyLocation)}) ${convoyLocation.region}`
    : convoyLocation.region;

  const destinationLabel = !compareBoards(location, destination)
    ? `(${getBoardName(destination)}) ${destination.region}`
    : destination.region;

  return (
    <div className="flex flex-row items-center gap-2">
      <UnitIcon unit={unit} variant="overlay" />
      <p>{location.region}</p>
      <div className="w-8 pl-1">
        <OrderPath
          position="relative"
          endType="none"
          pathDescription="M 0,0 32,0"
          dashPattern="1 20"
          width={10}
        />
      </div>
      {convoyedUnit && <UnitIcon unit={convoyedUnit} variant="overlay" />}
      <p>{convoyLocationLabel}</p>
      <div className="w-8">
        <OrderPath position="relative" endType="arrow" pathDescription="M 0,0 24,0" />
      </div>
      <p>{destinationLabel}</p>
    </div>
  );
};

export default ConvoyLabel;
