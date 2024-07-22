import Location, { compareLocations, getCoordinates } from '../../../types/location';
import OrderPath from './OrderPath';
import { orderArrowEndSeparation, orderArrowStartSeparation } from '../../../utils/constants';
import { OrderStatus } from '../../../types/order';

type SupportProps = {
  location: Location;
  status: OrderStatus;
  supportLocation?: Location | null;
  destination?: Location | null;
  isHighlighted?: boolean;
};

type HoldSupportProps = Omit<SupportProps, 'supportLocation'> & { destination: Location };
type MoveSupportProps = SupportProps & { supportLocation: Location; destination: Location };

const HoldSupport = ({ location, status, destination, isHighlighted }: HoldSupportProps) => {
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
    x: end.x - orderArrowStartSeparation * vectorNormed.x,
    y: end.y - orderArrowStartSeparation * vectorNormed.y,
  };

  return (
    <OrderPath
      pathDescription={`M ${startAdjusted.x},${startAdjusted.y} ${endAdjusted.x},${endAdjusted.y}`}
      endType="ring"
      dashPattern="2 10"
      status={status}
      isHighlighted={isHighlighted}
    />
  );
};

const MoveSupport = ({
  location,
  status,
  supportLocation,
  destination,
  isHighlighted,
}: MoveSupportProps) => {
  const start = getCoordinates(location);
  const mid = getCoordinates(supportLocation);
  const end = getCoordinates(destination);

  const startVector = {
    x: mid.x - start.x,
    y: mid.y - start.y,
  };
  const startNorm = Math.sqrt(startVector.x ** 2 + startVector.y ** 2);
  const startVectorNormed = {
    x: startVector.x / startNorm,
    y: startVector.y / startNorm,
  };

  const endVector = {
    x: end.x - mid.x,
    y: end.y - mid.y,
  };
  const endNorm = Math.sqrt(endVector.x ** 2 + endVector.y ** 2);
  const endVectorNormed = {
    x: endVector.x / endNorm,
    y: endVector.y / endNorm,
  };

  const startAdjusted = {
    x: start.x + orderArrowStartSeparation * startVectorNormed.x,
    y: start.y + orderArrowStartSeparation * startVectorNormed.y,
  };
  const endAdjusted = {
    x: end.x - orderArrowEndSeparation * endVectorNormed.x,
    y: end.y - orderArrowEndSeparation * endVectorNormed.y,
  };

  return (
    <OrderPath
      pathDescription={`M ${startAdjusted.x},${startAdjusted.y} Q ${mid.x},${mid.y} ${endAdjusted.x},${endAdjusted.y}`}
      endType="arrow"
      dashPattern="2 10"
      status={status}
      isHighlighted={isHighlighted}
    />
  );
};

const Support = ({
  location,
  status,
  supportLocation,
  destination,
  isHighlighted,
}: SupportProps) => {
  if (!supportLocation || !destination) return null;

  return compareLocations(supportLocation, destination) ? (
    <HoldSupport
      location={location}
      status={status}
      destination={destination}
      isHighlighted={isHighlighted}
    />
  ) : (
    <MoveSupport
      location={location}
      status={status}
      supportLocation={supportLocation}
      destination={destination}
      isHighlighted={isHighlighted}
    />
  );
};

export default Support;
