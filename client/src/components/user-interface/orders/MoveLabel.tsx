import { compareBoards, getBoardName } from '../../../types/board';
import Location from '../../../types/location';
import Unit from '../../../types/unit';
import UnitIcon from '../../world/boards/UnitIcon';
import OrderPath from '../../world/orders/OrderPath';

type MoveLabelProps = {
  unit: Unit | null;
  location: Location;
  destination?: Location | null;
};

const MoveLabel = ({ unit, location, destination }: MoveLabelProps) => {
  if (!unit || !destination) return null;

  const destinationLabel = !compareBoards(location, destination)
    ? `(${getBoardName(destination)}) ${destination.region}`
    : destination.region;

  return (
    <div className="flex flex-row items-center gap-2">
      <UnitIcon unit={unit} variant="overlay" />
      <p>{location.region}</p>
      <div className="w-8">
        <OrderPath position="relative" endType="arrow" pathDescription="M 0,0 24,0" />
      </div>
      <p>{destinationLabel}</p>
    </div>
  );
};

export default MoveLabel;
