import { useContext, useMemo } from 'react';
import { getNextMajorPhase, getPhaseIndex } from '../../../types/enums/phase';
import { filterUnique } from '../../../utils/listUtils';
import Board from './Board';
import BoardSkip from './BoardSkip';
import WorldContext from '../../context/WorldContext';
import BoardGhost from './BoardGhost';
import OrderEntryContext from '../../context/OrderEntryContext';
import InputMode from '../../../types/enums/inputMode';
import WorkQueueContext, { createWorkQueue, WorkQueue } from '../../context/WorkQueueContext';

const BoardLayer = () => {
  const { world } = useContext(WorldContext);
  const { currentMode, currentOrder } = useContext(OrderEntryContext);

  // Rasterisation shares a queue, this allows one item to be resolved at a time and displayed incrementally for better UX.
  // Without the queue, we'd try and rasterise everything at once which chugs the browser for a while.
  const rasteriseQueue = useMemo<WorkQueue>(createWorkQueue, []);

  if (!world) return null;

  const timelines = filterUnique(world.boards.map(({ timeline }) => timeline));
  const boards = filterUnique(world.boards.map(({ year, phase }) => ({ year, phase })));

  const { winner } = world;

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
    <div className="flex flex-col w-screen h-screen ">
      <WorkQueueContext.Provider value={rasteriseQueue}>
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

              if (!possibleBoard) return <BoardSkip key={key} board={{ ...board, timeline }} />;

              const hasNextBoard = world.boards.some(
                (worldBoard) =>
                  worldBoard.timeline === timeline &&
                  (worldBoard.year > board.year ||
                    (worldBoard.year === board.year &&
                      getPhaseIndex(worldBoard.phase) > getPhaseIndex(board.phase))),
              );

              return (
                <Board key={key} board={possibleBoard} winner={winner} isActive={!hasNextBoard} />
              );
            })}
            {timeline === selectedLocation?.timeline && ghostBoard}
          </div>
        ))}
      </WorkQueueContext.Provider>
    </div>
  );
};

export default BoardLayer;
