import { getOrderColour, OrderStatus } from '../../../types/order';
import colours from '../../../utils/colours';

type OrderPathProps = {
  pathDescription: string;
  endType: 'arrow' | 'ring' | 'none';
  dashPattern?: string;
  width?: number;
  status?: OrderStatus;
  isHighlighted?: boolean;
  zIndex?: number;
  position?: 'fixed' | 'relative';
};

const markerDefinitions = (
  <defs>
    <marker id="newArrowHead" className="overflow-visible" orient="auto-start-reverse">
      <path
        d="M -1,0 L -2,2 L 1.5,0 L -2,-2 Z"
        fill={colours.orderNew}
        strokeWidth="0.5"
        stroke={colours.orderNew}
        strokeLinejoin="round"
      />
    </marker>
    <marker id="successArrowHead" className="overflow-visible" orient="auto-start-reverse">
      <path
        d="M -1,0 L -2,2 L 1.5,0 L -2,-2 Z"
        fill={colours.orderSuccess}
        strokeWidth="0.5"
        stroke={colours.orderSuccess}
        strokeLinejoin="round"
      />
    </marker>
    <marker
      id="failureArrowHead"
      className="overflow-visible"
      orient="auto-start-reverse"
      strokeWidth="0.5"
      stroke={colours.orderFailure}
      strokeLinejoin="round"
    >
      <path d="M -1,0 L -2,2 L 1.5,0 L -2,-2 Z" fill={colours.orderFailure} />
    </marker>
    <marker
      id="retreatArrowHead"
      className="overflow-visible"
      orient="auto-start-reverse"
      strokeWidth="0.5"
      stroke={colours.orderRetreat}
      strokeLinejoin="round"
    >
      <path d="M -1,0 L -2,2 L 1.5,0 L -2,-2 Z" fill={colours.orderRetreat} />
    </marker>
    <marker
      id="highlightArrowHead"
      className="overflow-visible"
      orient="auto-start-reverse"
      strokeWidth="0.5"
      stroke={colours.orderHighlight}
      strokeLinejoin="round"
    >
      <path d="M -1,0 L -2,2 L 1.5,0 L -2,-2 Z" fill={colours.orderHighlight} />
    </marker>
    <marker refX="-5" id="newSupportRing" className="overflow-visible" orient="auto-start-reverse">
      <circle
        cx="0"
        cy="0"
        r="5"
        fill="none"
        stroke={colours.orderNew}
        strokeDasharray="0.5 3"
        strokeLinecap="round"
      />
    </marker>
    <marker
      refX="-5"
      id="successSupportRing"
      className="overflow-visible"
      orient="auto-start-reverse"
    >
      <circle
        cx="0"
        cy="0"
        r="5"
        fill="none"
        stroke={colours.orderSuccess}
        strokeDasharray="0.5 3"
        strokeLinecap="round"
      />
    </marker>
    <marker
      refX="-5"
      id="failureSupportRing"
      className="overflow-visible"
      orient="auto-start-reverse"
    >
      <circle
        cx="0"
        cy="0"
        r="5"
        fill="none"
        stroke={colours.orderFailure}
        strokeDasharray="0.5 3"
        strokeLinecap="round"
      />
    </marker>
    <marker
      refX="-5"
      id="highlightSupportRing"
      className="overflow-visible"
      orient="auto-start-reverse"
    >
      <circle
        cx="0"
        cy="0"
        r="5"
        fill="none"
        stroke={colours.orderFailure}
        strokeDasharray="0.5 3"
        strokeLinecap="round"
      />
    </marker>
  </defs>
);

const OrderPath = ({
  pathDescription,
  endType,
  dashPattern,
  width = 4,
  status = OrderStatus.New,
  isHighlighted,
  zIndex = 10,
  position = 'fixed',
}: OrderPathProps) => {
  const newMarkerId = {
    arrow: 'newArrowHead',
    ring: 'newSupportRing',
    none: '',
  }[endType];
  const successMarkerId = {
    arrow: 'successArrowHead',
    ring: 'successSupportRing',
    none: '',
  }[endType];
  const failureMarkerId = {
    arrow: 'failureArrowHead',
    ring: 'failureSupportRing',
    none: '',
  }[endType];
  const retreatMarkerId = {
    arrow: 'retreatArrowHead',
    ring: '',
    none: '',
  }[endType];
  const highlightMarkerId = {
    arrow: 'highlightArrowHead',
    ring: 'highlightSupportRing',
    none: '',
  }[endType];

  const markerId = isHighlighted
    ? highlightMarkerId
    : {
        [OrderStatus.New]: newMarkerId,
        [OrderStatus.Success]: successMarkerId,
        [OrderStatus.Failure]: failureMarkerId,
        [OrderStatus.Invalid]: failureMarkerId,
        [OrderStatus.Retreat]: retreatMarkerId,
      }[status];

  return (
    <svg
      className="overflow-visible pointer-events-none"
      style={{ zIndex, position }}
      height={1}
      width={1}
    >
      {markerDefinitions}
      <path
        stroke={getOrderColour(status, isHighlighted)}
        fill="none"
        strokeWidth={width}
        d={pathDescription}
        markerEnd={`url(#${markerId})`}
        strokeDasharray={dashPattern}
        strokeLinecap="round"
      />
    </svg>
  );
};

export default OrderPath;
