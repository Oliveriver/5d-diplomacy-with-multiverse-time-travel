import Location, { getCoordinates } from '../../../types/location';
import { OrderStatus } from '../../../types/order';
import { orderArrowEndSeparation, orderArrowStartSeparation } from '../../../utils/constants';
import OrderPath from './OrderPath';

type MoveProps = {
  location: Location;
  status: OrderStatus;
  destination?: Location | null;
  isHighlighted?: boolean;
};

const Move = ({ location, destination, status, isHighlighted }: MoveProps) => {
  if (!destination) return null;

  const start = getCoordinates(location);
  const end = getCoordinates(destination);

  const vector = {
    x: end.x - start.x,
    y: end.y - start.y,
  };
  const norm = Math.sqrt(vector.x ** 2 + vector.y ** 2);
  const vectorNormed = {
    x: vector.x / norm,
    y: vector.y / norm,
  };

  const startAdjusted = {
    x: start.x + orderArrowStartSeparation * vectorNormed.x,
    y: start.y + orderArrowStartSeparation * vectorNormed.y,
  };
  const endAdjusted = {
    x: end.x - orderArrowEndSeparation * vectorNormed.x,
    y: end.y - orderArrowEndSeparation * vectorNormed.y,
  };

  return (
    <OrderPath
      pathDescription={`M ${startAdjusted.x},${startAdjusted.y} ${endAdjusted.x},${endAdjusted.y}`}
      endType="arrow"
      status={status}
      isHighlighted={isHighlighted}
      zIndex={20}
    />
  );
};

export default Move;
