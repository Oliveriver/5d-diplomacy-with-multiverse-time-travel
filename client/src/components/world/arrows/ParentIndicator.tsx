import { majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import Location from '../../../types/location';
import Phase from '../../../types/enums/phase';
import colours from '../../../utils/colours';

type ParentIndicatorProps = {
  location: Omit<Location, 'region'>;
  parentTimeline?: number;
  x: number;
  y: number;
};

const ParentIndicator = ({ location, parentTimeline, x, y }: ParentIndicatorProps) => {
  if (!parentTimeline) return null;

  const width = location.phase === Phase.Winter ? minorBoardWidth : majorBoardWidth;

  return (
    <div className="fixed z-10 -mt-24" style={{ top: y - width / 2, left: x - width / 2 }}>
      <svg className="absolute overflow-visible">
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
        <path strokeWidth={20} fill="none" d="M 0,0 -10,-10" markerEnd="url(#boardArrowhead)" />
      </svg>
      <p className="text-7xl font-bold" style={{ color: colours.boardArrowHead }}>
        {parentTimeline}
      </p>
    </div>
  );
};

export default ParentIndicator;
