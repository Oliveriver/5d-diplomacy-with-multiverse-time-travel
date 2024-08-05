import { useContext } from 'react';
import { useTransformContext } from 'react-zoom-pan-pinch';
import OrderEntryContext from '../context/OrderEntryContext';
import Order, { getOrderText, OrderType } from '../../types/order';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';
import { getCoordinates, getLocationKey } from '../../types/location';
import { orderFocusScale } from '../../utils/constants';
import WorldContext from '../context/WorldContext';
import RemoveButton from './common/RemoveButton';
import HoldLabel from './orders/HoldLabel';
import MoveLabel from './orders/MoveLabel';
import SupportLabel from './orders/SupportLabel';
import ConvoyLabel from './orders/ConvoyLabel';
import BuildLabel from './orders/BuildLabel';
import DisbandLabel from './orders/DisbandLabel';

type OrderListItemProps = {
  order: Order;
};

const OrderListItem = ({ order }: OrderListItemProps) => {
  const { isLoading } = useContext(WorldContext);

  const { dispatch } = useContext(OrderEntryContext);
  const { setTransformState } = useTransformContext();

  const { $type, location } = order;

  const moveToOrder = () => {
    const coordinates = getCoordinates(location);
    const offsetX = window.innerWidth / 2 - coordinates.x * orderFocusScale;
    const offsetY = window.innerHeight / 2 - coordinates.y * orderFocusScale;
    setTransformState(orderFocusScale, offsetX, offsetY);
  };

  const highlightStart = () =>
    dispatch({
      $type: OrderEntryActionType.HighlightStart,
      location,
    });

  const highlightStop = () => dispatch({ $type: OrderEntryActionType.HighlightStop });

  const remove = () =>
    dispatch({
      $type: OrderEntryActionType.Remove,
      location,
    });

  const label = {
    [OrderType.Hold]: <HoldLabel {...order} />,
    [OrderType.Move]: <MoveLabel {...order} />,
    [OrderType.Support]: <SupportLabel {...order} />,
    [OrderType.Convoy]: <ConvoyLabel {...order} />,
    [OrderType.Build]: <BuildLabel {...order} />,
    [OrderType.Disband]: <DisbandLabel {...order} />,
  }[$type];

  return (
    <div
      key={getLocationKey(location)}
      className="flex flex-row items-center"
      onMouseEnter={highlightStart}
      onMouseLeave={highlightStop}
    >
      <button
        title={getOrderText(order)}
        type="button"
        className="cursor-pointer"
        onClick={moveToOrder}
      >
        {label}
      </button>
      <RemoveButton remove={remove} isDisabled={isLoading} />
    </div>
  );
};

export default OrderListItem;
