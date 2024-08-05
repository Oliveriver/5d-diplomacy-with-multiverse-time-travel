import { useContext } from 'react';
import { getNextMajorPhase, getNextPhase } from '../../../types/enums/phase';
import { filterUnique } from '../../../utils/listUtils';
import Board from './Board';
import BoardSkip from './BoardSkip';
import WorldContext from '../../context/WorldContext';
import BoardGhost from './BoardGhost';
import OrderEntryContext from '../../context/OrderEntryContext';
import InputMode from '../../../types/enums/inputMode';

const BoardLayer = () => {
  const { world } = useContext(WorldContext);
  const { currentMode, currentOrder } = useContext(OrderEntryContext);
  if (!world) return null;

  const timelines = filterUnique(world.boards.map(({ timeline }) => timeline));
  const boards = filterUnique(world.boards.map(({ year, phase }) => ({ year, phase })));

  const { winner } = world;
  const hasRetreats = world.boards.some((board) =>
    Object.values(board.units).some((unit) => unit.mustRetreat),
  );

  const selectedLocation = currentOrder?.location;
  const showGhostBoard =
    (currentMode === InputMode.Support || currentMode === InputMode.Convoy) && selectedLocation;
  const ghostBoard = showGhostBoard && (
    <BoardGhost
      initialTimeline={selectedLocation.timeline}
      initialYear={getNextMajorPhase(selectedLocation.year, selectedLocation.phase).year}
      initialPhase={getNextMajorPhase(selectedLocation.year, selectedLocation.phase).phase}
    />
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
          {timeline === selectedLocation?.timeline && ghostBoard}
        </div>
      ))}
    </div>
  );
};

export default BoardLayer;
