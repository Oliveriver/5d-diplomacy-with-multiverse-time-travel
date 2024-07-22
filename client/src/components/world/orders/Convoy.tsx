import Location, { getCoordinates } from '../../../types/location';
import OrderPath from './OrderPath';
import { orderArrowEndSeparation, orderArrowStartSeparation } from '../../../utils/constants';
import { OrderStatus } from '../../../types/order';

type ConvoyProps = {
  location: Location;
  status: OrderStatus;
  convoyLocation?: Location | null;
  destination?: Location | null;
  isHighlighted?: boolean;
};

const Convoy = ({ location, status, convoyLocation, destination, isHighlighted }: ConvoyProps) => {
  if (!convoyLocation || !destination) return null;

  const start = getCoordinates(convoyLocation);
  const mid = getCoordinates(location);
  const end = getCoordinates(destination);
  const connection = {
    x: (start.x + end.x) / 2,
    y: (start.y + end.y) / 2,
  };

  const moveVector = {
    x: end.x - start.x,
    y: end.y - start.y,
  };
  const moveNorm = Math.sqrt(moveVector.x ** 2 + moveVector.y ** 2);
  const moveVectorNormed = {
    x: moveVector.x / moveNorm,
    y: moveVector.y / moveNorm,
  };

  const convoyVector = {
    x: connection.x - mid.x,
    y: connection.y - mid.y,
  };
  const convoyNorm = Math.sqrt(convoyVector.x ** 2 + convoyVector.y ** 2);
  const convoyVectorNormed = {
    x: convoyVector.x / convoyNorm,
    y: convoyVector.y / convoyNorm,
  };

  const moveStartAdjusted = {
    x: start.x + orderArrowStartSeparation * moveVectorNormed.x,
    y: start.y + orderArrowStartSeparation * moveVectorNormed.y,
  };
  const moveEndAdjusted = {
    x: end.x - orderArrowEndSeparation * moveVectorNormed.x,
    y: end.y - orderArrowEndSeparation * moveVectorNormed.y,
  };

  const convoyStartAdjusted = {
    x: mid.x + orderArrowStartSeparation * convoyVectorNormed.x,
    y: mid.y + orderArrowStartSeparation * convoyVectorNormed.y,
  };
  const convoyEndAdjusted = {
    x: connection.x - orderArrowEndSeparation * convoyVectorNormed.x,
    y: connection.y - orderArrowEndSeparation * convoyVectorNormed.y,
  };

  return (
    <>
      <OrderPath
        pathDescription={`M ${moveStartAdjusted.x},${moveStartAdjusted.y} ${moveEndAdjusted.x},${moveEndAdjusted.y}`}
        endType="none"
        dashPattern="1 20"
        width={10}
        status={status}
        isHighlighted={isHighlighted}
      />
      <OrderPath
        pathDescription={`M ${convoyStartAdjusted.x},${convoyStartAdjusted.y} ${convoyEndAdjusted.x},${convoyEndAdjusted.y}`}
        endType="none"
        status={status}
        isHighlighted={isHighlighted}
      />
    </>
  );
};

export default Convoy;
