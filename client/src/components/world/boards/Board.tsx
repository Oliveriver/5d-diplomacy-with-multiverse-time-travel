import Phase from '../../../types/enums/phase';
import {
  boardBorderWidth,
  boardSeparation,
  majorBoardWidth,
  minorBoardWidth,
} from '../../../utils/constants';
import Map from './Map';
import BoardData, { getBoardKey, getBoardName } from '../../../types/board';
import Adjustment from './Adjustment';
import Nation, { getNationColour } from '../../../types/enums/nation';
import colours from '../../../utils/colours';

type BoardProps = {
  board: BoardData;
  isActive: boolean;
  winner: Nation | null;
};

const Board = ({ board, isActive, winner }: BoardProps) => {
  const { phase } = board;
  const showWinner = isActive && winner;

  const width = phase === Phase.Winter ? minorBoardWidth : majorBoardWidth;
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
        className="relative rounded-xl"
        style={{
          backgroundColor: colours.boardBackground,
          minWidth: width,
          width,
          minHeight: width,
          height: width,
          borderWidth: boardBorderWidth,
          borderColor: showWinner ? getNationColour(winner) : colours.boardBorder,
          boxShadow: showWinner ? `0px 0px 100px 50px ${getNationColour(winner)}` : '',
        }}
      >
        <p className="text-md -mt-7">{getBoardName(board)}</p>
        <Map board={board} isActive={isActive && !winner} />
      </div>
      {phase === Phase.Winter && <Adjustment board={board} />}
    </div>
  );
};

export default Board;
