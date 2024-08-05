import { useContext } from 'react';
import { useTransformContext } from 'react-zoom-pan-pinch';
import OrderEntryContext from '../context/OrderEntryContext';
import Order, { displayOrder } from '../../types/order';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';
import { getNationColour } from '../../types/enums/nation';
import { getCoordinates, getLocationKey } from '../../types/location';
import { orderFocusScale } from '../../utils/constants';
import WorldContext from '../context/WorldContext';
import RemoveButton from './common/RemoveButton';

type OrderListItemProps = {
  order: Order;
};

// TODO prettify
const OrderListItem = ({ order }: OrderListItemProps) => {
  const { isLoading } = useContext(WorldContext);

  const { dispatch } = useContext(OrderEntryContext);
  const { setTransformState } = useTransformContext();

  const moveToOrder = () => {
    const coordinates = getCoordinates(order.location);
    const offsetX = window.innerWidth / 2 - coordinates.x * orderFocusScale;
    const offsetY = window.innerHeight / 2 - coordinates.y * orderFocusScale;
    setTransformState(orderFocusScale, offsetX, offsetY);
  };

  const highlightStart = () =>
    dispatch({
      $type: OrderEntryActionType.HighlightStart,
      location: order.location,
    });

  const highlightStop = () => dispatch({ $type: OrderEntryActionType.HighlightStop });

  const remove = () =>
    dispatch({
      $type: OrderEntryActionType.Remove,
      location: order.location,
    });

  return (
    <div
      key={getLocationKey(order.location)}
      className="flex flex-row items-center"
      onMouseEnter={highlightStart}
      onMouseLeave={highlightStop}
    >
      <button
        type="button"
        className="cursor-pointer"
        style={{ color: getNationColour(order.unit?.owner) }}
        onClick={moveToOrder}
      >
        {displayOrder(order)}
      </button>
      <RemoveButton remove={remove} isDisabled={isLoading} />
    </div>
  );
};

export default OrderListItem;
