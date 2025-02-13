import { useContext } from 'react';
import { getNextMajorPhase, getPhaseIndex } from '../../../types/enums/phase';
import { filterUnique } from '../../../utils/listUtils';
import Board from './Board';
import BoardSkip from './BoardSkip';
import WorldContext from '../../context/WorldContext';
import BoardGhost from './BoardGhost';
import OrderEntryContext from '../../context/OrderEntryContext';
import InputMode from '../../../types/enums/inputMode';
import colours from '../../../utils/colours';

const BoardLayer = () => {
  const { world } = useContext(WorldContext);
  const { currentMode, currentOrder } = useContext(OrderEntryContext);
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
    <div
      className="flex flex-col w-screen h-screen dark:[&_g]:stroke-black dark:[&_path]:stroke-black dark:[&_path.sea-region]:fill-[var(--sea-colour)] dark:[&_g.supply-center-dot]:fill-[var(--supply-centre-colour)]"
      style={{
        '--sea-colour': colours.sea,
        '--supply-centre-colour': colours.uiForeground,
      } as React.CSSProperties}
    >
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
    </div>
  );
};

export default BoardLayer;
