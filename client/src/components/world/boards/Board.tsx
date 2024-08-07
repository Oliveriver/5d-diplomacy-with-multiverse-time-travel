import Phase from '../../../types/enums/phase';
import {
  boardBorderWidth,
  boardSeparation,
  majorBoardWidth,
  minorBoardWidth,
} from '../../../utils/constants';
import Map from './Map';
import BoardData, { getBoardName } from '../../../types/board';
import Adjustment from './Adjustment';
import Nation, { getNationColour } from '../../../types/enums/nation';
import colours from '../../../utils/colours';

type BoardProps = {
  board: BoardData;
  winner: Nation | null;
};

const Board = ({ board, winner }: BoardProps) => {
  const { phase } = board;

  const width = phase === Phase.Winter ? minorBoardWidth : majorBoardWidth;

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
        className="relative rounded-xl"
        style={{
          backgroundColor: colours.boardBackground,
          minWidth: width,
          width,
          minHeight: width,
          height: width,
          borderWidth: boardBorderWidth,
          borderColor: winner ? getNationColour(winner) : colours.boardBorder,
          boxShadow: winner ? `0px 0px 100px 50px ${getNationColour(winner)}` : '',
        }}
      >
        <p className="text-md -mt-7">{getBoardName(board)}</p>
        <Map board={board} />
      </div>
      {phase === Phase.Winter && <Adjustment board={board} />}
    </div>
  );
};

export default Board;
