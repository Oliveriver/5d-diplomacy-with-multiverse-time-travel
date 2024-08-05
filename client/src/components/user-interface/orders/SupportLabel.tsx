import { useContext } from 'react';
import Unit from '../../../types/unit';
import Location, { compareLocations } from '../../../types/location';
import OrderPath from '../../world/orders/OrderPath';
import WorldContext from '../../context/WorldContext';
import UnitIcon from '../../world/boards/UnitIcon';
import { compareBoards, getBoardName } from '../../../types/board';
import { findUnit } from '../../../types/world';

type SupportLabelProps = {
  unit: Unit | null;
  location: Location;
  supportLocation?: Location | null;
  destination?: Location | null;
};

const SupportLabel = ({ unit, location, supportLocation, destination }: SupportLabelProps) => {
  const { world } = useContext(WorldContext);

  if (!unit || !supportLocation || !destination) return null;

  const supportedUnit = findUnit(world, supportLocation);

  const isMoveSupport = !compareLocations(supportLocation, destination);

  const supportLocationLabel = !compareBoards(location, supportLocation)
    ? `(${getBoardName(supportLocation)}) ${supportLocation.region}`
    : supportLocation.region;

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
          dashPattern="2 10"
        />
      </div>
      {supportedUnit && <UnitIcon unit={supportedUnit} variant="overlay" />}
      {isMoveSupport && (
        <>
          <p>{supportLocationLabel}</p>
          <div className="w-8">
            <OrderPath position="relative" endType="arrow" pathDescription="M 0,0 24,0" />
          </div>
        </>
      )}
      <p>{destinationLabel}</p>
    </div>
  );
};

export default SupportLabel;
