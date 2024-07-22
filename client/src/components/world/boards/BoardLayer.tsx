import { useContext } from 'react';
import { getNextPhase } from '../../../types/enums/phase';
import { filterUnique } from '../../../utils/listUtils';
import Board from './Board';
import BoardSkip from './BoardSkip';
import WorldContext from '../../context/WorldContext';

const BoardLayer = () => {
  const { world } = useContext(WorldContext);
  if (!world) return null;

  const timelines = filterUnique(world.boards.map(({ timeline }) => timeline));
  const boards = filterUnique(world.boards.map(({ year, phase }) => ({ year, phase })));

  const { winner } = world;
  const hasRetreats = world.boards.some((board) =>
    Object.values(board.units).some((unit) => unit.mustRetreat),
  );

  return (
    <div className="flex flex-col w-screen h-screen">
      {timelines.map((timeline) => (
        <div className="flex" key={timeline}>
          {boards.map((board) => {
            const { year, phase } = board;
            const possibleBoard = world.boards.find(
              (worldBoard) =>
                worldBoard.timeline === timeline &&
                worldBoard.year === year &&
                worldBoard.phase === phase,
            );

            const key = `${board.year}-${board.phase}`;
            const nextBoard = getNextPhase(year, phase);
            const hasNextBoard = world.boards.find(
              (otherBoard) =>
                otherBoard.timeline === timeline &&
                otherBoard.year === nextBoard.year &&
                otherBoard.phase === nextBoard.phase,
            );

            return possibleBoard ? (
              <Board
                key={key}
                board={possibleBoard}
                isActive={!hasNextBoard && !hasRetreats}
                winner={winner}
              />
            ) : (
              <BoardSkip key={key} board={{ ...board, timeline }} />
            );
          })}
        </div>
      ))}
    </div>
  );
};

export default BoardLayer;
