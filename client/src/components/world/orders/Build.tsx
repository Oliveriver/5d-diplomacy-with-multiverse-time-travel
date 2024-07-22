import Location, { getCoordinates } from '../../../types/location';
import { getOrderColour, OrderStatus } from '../../../types/order';
import Unit from '../../../types/unit';
import { boardBorderWidth, majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import UnitIcon from '../boards/UnitIcon';

type BuildProps = {
  location: Location;
  status: OrderStatus;
  unit?: Unit | null;
  isHighlighted?: boolean;
};

const Build = ({ location, status, unit, isHighlighted }: BuildProps) => {
  if (!unit) return null;

  const coordinates = getCoordinates(location);
  const unitScaleFactor =
    (minorBoardWidth - boardBorderWidth * 2) / (majorBoardWidth - boardBorderWidth * 2);
  const radius = 10;

  return (
    <>
      <svg className="z-10 fixed overflow-visible pointer-events-none">
        <path
          d={`M ${coordinates.x},${coordinates.y - radius} ${coordinates.x},${coordinates.y + radius}`}
          stroke={getOrderColour(status, isHighlighted)}
          strokeLinecap="round"
          strokeWidth={8}
        />
        <path
          d={`M ${coordinates.x - radius},${coordinates.y} ${coordinates.x + radius},${coordinates.y}`}
          stroke={getOrderColour(status, isHighlighted)}
          strokeLinecap="round"
          strokeWidth={8}
        />
      </svg>
      <div className="fixed z-20" style={{ left: coordinates.x, top: coordinates.y }}>
        <UnitIcon unit={unit} scaleFactor={unitScaleFactor} />
      </div>
    </>
  );
};

export default Build;
