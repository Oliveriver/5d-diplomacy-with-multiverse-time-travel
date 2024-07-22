import { useContext, useRef } from 'react';
import OrderEntryContext from '../context/OrderEntryContext';
import OrderListItem from './OrderListItem';
import { getOrderKey } from '../../types/order';
import colours from '../../utils/colours';
import { filterUnique } from '../../utils/listUtils';

const OrderList = () => {
  const { orders } = useContext(OrderEntryContext);
  const scrollRef = useRef<HTMLDivElement>(null);

  if (orders.length === 0) return null;

  const timelines = filterUnique(orders.map(({ location }) => location.timeline)).sort();

  const maxHeight = window.innerHeight - 168;
  const scrollBehaviour =
    scrollRef.current && scrollRef.current.scrollHeight > maxHeight ? 'scroll' : 'hidden';

  return (
    <div
      ref={scrollRef}
      className="absolute right-10 bottom-32 flex flex-col gap-4 items-end"
      style={{
        maxHeight,
        overflowY: scrollBehaviour,
      }}
    >
      {timelines.map((timeline) => (
        <div
          key={timeline}
          className="rounded p-4 flex flex-col gap-2 items-end"
          style={{ backgroundColor: colours.uiOverlay }}
        >
          <p className="self-start opacity-70 text-sm">{`Timeline ${timeline}`}</p>
          {orders
            .filter((order) => order.location.timeline === timeline)
            .map((order) => (
              <OrderListItem key={getOrderKey(order)} order={order} />
            ))}
        </div>
      ))}
    </div>
  );
};

export default OrderList;
