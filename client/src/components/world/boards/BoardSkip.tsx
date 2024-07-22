import Phase from '../../../types/enums/phase';
import { boardSeparation, majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import Location from '../../../types/location';
import { getBoardKey } from '../../../types/board';

type BoardGapProps = {
  board: Omit<Location, 'region'>;
};

const BoardSkip = ({ board }: BoardGapProps) => {
  const width = board.phase === Phase.Winter ? minorBoardWidth : majorBoardWidth;
  const key = getBoardKey(board);

  return (
    <div
      className="flex-col content-center relative"
      style={{
        minHeight: majorBoardWidth,
        height: majorBoardWidth,
        margin: boardSeparation / 2,
      }}
    >
      <div
        id={key}
        className="relative"
        style={{
          minWidth: width,
          width,
          minHeight: width,
          height: width,
        }}
      />
    </div>
  );
};

export default BoardSkip;
