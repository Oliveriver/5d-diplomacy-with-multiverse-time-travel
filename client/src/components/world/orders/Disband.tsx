import Location, { getCoordinates } from '../../../types/location';
import { getOrderColour, OrderStatus } from '../../../types/order';

type DisbandProps = {
  location: Location;
  status: OrderStatus;
  isHighlighted?: boolean;
};

const Disband = ({ location, status, isHighlighted }: DisbandProps) => {
  const coordinates = getCoordinates(location);
  const radius = 10;

  return (
    <svg className="z-20 fixed overflow-visible pointer-events-none">
      <path
        d={`M ${coordinates.x - radius},${coordinates.y} ${coordinates.x + radius},${coordinates.y}`}
        stroke={getOrderColour(status, isHighlighted)}
        strokeLinecap="round"
        strokeWidth={8}
      />
    </svg>
  );
};

export default Disband;
