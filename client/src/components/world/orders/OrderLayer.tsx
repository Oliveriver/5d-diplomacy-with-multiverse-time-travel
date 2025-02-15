import { useContext } from 'react';
import { OrderType, getOrderKey } from '../../../types/order';
import Move from './Move';
import Convoy from './Convoy';
import Support from './Support';
import Build from './Build';
import Disband from './Disband';
import OrderEntryContext from '../../context/OrderEntryContext';
import WorldContext from '../../context/WorldContext';
import { pastTurnOpacity } from '../../../utils/constants';

const OrderLayer = () => {
  const { world } = useContext(WorldContext);
  const { orders, highlightedOrder, currentOrder } = useContext(OrderEntryContext);
  if (!world) return null;

  const pastOrderOpacity =
    currentOrder !== null && currentOrder.$type !== OrderType.Build ? 1 : pastTurnOpacity;

  return (
    <>
      <div className="absolute z-50" style={{ opacity: pastOrderOpacity }}>
        {world.orders.map((order) => {
          const key = getOrderKey(order);
          const isHighlighted = highlightedOrder !== null && key === getOrderKey(highlightedOrder);

          return {
            [OrderType.Hold]: null,
            [OrderType.Move]: <Move key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Support]: <Support key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Convoy]: <Convoy key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Build]: <Build key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Disband]: <Disband key={key} isHighlighted={isHighlighted} {...order} />,
          }[order.$type];
        })}
      </div>
      <>
        {orders.map((order) => {
          const key = getOrderKey(order);
          const isHighlighted = highlightedOrder !== null && key === getOrderKey(highlightedOrder);

          return {
            [OrderType.Hold]: null,
            [OrderType.Move]: <Move key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Support]: <Support key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Convoy]: <Convoy key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Build]: <Build key={key} isHighlighted={isHighlighted} {...order} />,
            [OrderType.Disband]: <Disband key={key} isHighlighted={isHighlighted} {...order} />,
          }[order.$type];
        })}
      </>
    </>
  );
};

export default OrderLayer;
