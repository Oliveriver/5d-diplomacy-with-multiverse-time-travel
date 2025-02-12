import { useContext } from 'react';
import WorldContext from '../context/WorldContext';
import { filterUnique } from '../../utils/listUtils';
import colours from '../../utils/colours';
import { boardSeparation, majorBoardWidth, minorBoardWidth } from '../../utils/constants';

const BackgroundLayer = () => {
  const { world } = useContext(WorldContext);
  if (!world) return null;

  const segmentWidth = 2 * majorBoardWidth + minorBoardWidth + 3 * boardSeparation;
  const segmentHeight =
    (majorBoardWidth + boardSeparation) *
    (2 + Math.max(...world.boards.map((board) => board.timeline)));

  const years = filterUnique(world.boards.map(({ year }) => year));

  return (
    <div className="absolute flex">
      {years.map((year) => (
        <div
          key={year}
          className="border-l-[48px] border-dashed flex"
          style={{
            width: segmentWidth,
            height: segmentHeight,
            marginTop: -(majorBoardWidth + boardSeparation),
            borderColor: colours.yearDivider,
          }}
        >
          <p
            className="text-[384px] font-bold rotate-90 self-start mt-24 leading-tight -ml-24"
            style={{ color: colours.yearDividerText }}
          >
            {year}
          </p>
        </div>
      ))}
    </div>
  );
};

export default BackgroundLayer;
