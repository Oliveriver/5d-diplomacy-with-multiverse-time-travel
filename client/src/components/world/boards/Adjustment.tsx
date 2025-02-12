import Nation, { getNationColour } from '../../../types/enums/nation';
import Board from '../../../types/board';
import { boardBorderWidth, pastTurnOpacity } from '../../../utils/constants';
import SignIcon from '../../user-interface/common/SignIcon';

type AdjustmentProps = {
  board: Board;
  isActive: boolean;
};

const Adjustment = ({ board, isActive }: AdjustmentProps) => {
  const nations = Object.values(Nation);
  const centreOwners = Object.values(board.centres);
  const units = Object.values(board.units);

  const adjustments = nations.map((nation) => {
    const centreCount = centreOwners.filter((owner) => owner === nation).length;
    const unitCount = units.filter((unit) => unit.owner === nation).length;

    return centreCount - unitCount;
  });

  return (
    <div
      className="absolute w-full flex py-2"
      style={{
        paddingLeft: boardBorderWidth,
        paddingRight: boardBorderWidth,
        opacity: isActive ? 1 : pastTurnOpacity,
      }}
    >
      <div className="flex-grow flex-shrink flex flex-col gap-1">
        {adjustments.map((count, i) => {
          if (count <= 0) return null;

          return (
            <div key={nations[i]} className="flex gap-1">
              {[...Array(count)].map((_, j) => (
                // eslint-disable-next-line react/no-array-index-key
                <SignIcon key={j} type="plus" colour={getNationColour(nations[i])} />
              ))}
            </div>
          );
        })}
      </div>
      <div className="flex flex-col gap-1 mt-1">
        {adjustments.map((count, i) => {
          if (count >= 0) return null;

          return (
            <div key={nations[i]} className="flex gap-1 justify-end">
              {[...Array(-count)].map((_, j) => (
                // eslint-disable-next-line react/no-array-index-key
                <SignIcon key={j} type="minus" colour={getNationColour(nations[i])} />
              ))}
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default Adjustment;
