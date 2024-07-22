import { useContext } from 'react';
import { OrderType, getOrderKey } from '../../../types/order';
import Move from './Move';
import Convoy from './Convoy';
import Support from './Support';
import Build from './Build';
import Disband from './Disband';
import OrderEntryContext from '../../context/OrderEntryContext';
import WorldContext from '../../context/WorldContext';

const OrderLayer = () => {
  const { world } = useContext(WorldContext);
  const { orders, highlightedOrder } = useContext(OrderEntryContext);
  if (!world) return null;

  return (
    <>
      {[...world.orders, ...orders].map((order) => {
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
  );
};

export default OrderLayer;
