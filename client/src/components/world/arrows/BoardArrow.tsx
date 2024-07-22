import colours from '../../../utils/colours';
import { boardArrowWidth } from '../../../utils/constants';

type BoardArrowProps = {
  startX: number;
  endX: number;
  startY: number;
  endY: number;
  includeHead?: boolean;
};

const BoardArrow = ({ startX, endX, startY, endY, includeHead }: BoardArrowProps) => (
  <svg className="fixed overflow-visible">
    <defs>
      <marker id="boardArrowhead" className="overflow-visible" orient="auto-start-reverse">
        <path
          d="M 0,0 L -0.5,1 L 1.5,0 L -0.5,-1 Z"
          fill={colours.boardArrowHead}
          stroke={colours.boardArrowHead}
          strokeWidth={0.1}
          strokeLinejoin="round"
        />
      </marker>
    </defs>
    <path
      stroke={colours.boardArrowBody}
      strokeWidth={boardArrowWidth}
      fill="none"
      d={`M ${startX},${startY} C ${endX},${startY} ${startX},${endY} ${endX},${endY}`}
      markerEnd={includeHead ? 'url(#boardArrowhead)' : ''}
    />
  </svg>
);
export default BoardArrow;
