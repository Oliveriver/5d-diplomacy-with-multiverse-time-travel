import { useContext, useRef, useState } from 'react';
import OrderEntryContext from '../context/OrderEntryContext';
import OrderListItem from './OrderListItem';
import Order, { getOrderKey } from '../../types/order';
import colours from '../../utils/colours';
import { filterUnique } from '../../utils/listUtils';
import ExpandButton from './common/ExpandButton';

type OrderListTimelineProps = {
  timeline: number;
  orders: Order[];
};

const OrderListTimeline = ({ timeline, orders }: OrderListTimelineProps) => {
  const [isExpanded, setIsExpanded] = useState(true);

  return (
    <div
      className="rounded p-4 flex flex-col gap-2 items-end pointer-events-auto"
      style={{ backgroundColor: colours.uiOverlay }}
    >
      <div className="self-end flex flex-row items-center">
        <p className="opacity-70 text-sm">{`Timeline ${timeline}`}</p>
        <ExpandButton
          colour="black"
          isExpanded={isExpanded}
          toggleExpand={() => setIsExpanded(!isExpanded)}
        />
      </div>
      {isExpanded &&
        orders.map((order) => <OrderListItem key={getOrderKey(order)} order={order} />)}
    </div>
  );
};

const OrderList = () => {
  const { orders } = useContext(OrderEntryContext);
  const scrollRef = useRef<HTMLDivElement>(null);

  if (orders.length === 0) return null;

  const timelines = filterUnique(orders.map(({ location }) => location.timeline)).sort();

  const maxHeight = window.innerHeight - 168;

  return (
    <div
      ref={scrollRef}
      className="absolute right-10 bottom-32 flex flex-col gap-4 items-end pointer-events-none"
      style={{
        maxHeight,
        overflowY: 'auto',
      }}
    >
      {timelines.map((timeline) => (
        <OrderListTimeline
          key={timeline}
          timeline={timeline}
          orders={orders.filter((order) => order.location.timeline === timeline)}
        />
      ))}
    </div>
  );
};

export default OrderList;
