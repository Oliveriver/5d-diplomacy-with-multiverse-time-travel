import { useContext } from 'react';
import Phase, { getNextPhase, getPhaseIndex } from '../../../types/enums/phase';
import { filterUnique } from '../../../utils/listUtils';
import BoardArrow from './BoardArrow';
import { boardSeparation, majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import { getCoordinates } from '../../../types/location';
import WorldContext from '../../context/WorldContext';
import ParentIndicator from './ParentIndicator';

const BoardArrowLayer = () => {
  const { world } = useContext(WorldContext);
  if (!world) return null;

  const timelines = filterUnique(world.boards.map(({ timeline }) => timeline));

  const horizontalArrows = timelines.map((timeline) => {
    const boards = world.boards
      .filter((board) => board.timeline === timeline)
      .sort((board1, board2) => {
        if (board1.year !== board2.year) return board1.year - board2.year;
        return getPhaseIndex(board1.phase) - getPhaseIndex(board2.phase);
      });

    const earliestBoard = boards[0];
    const latestBoard = boards[boards.length - 1];

    const startCoordinates = getCoordinates({
      timeline: earliestBoard.timeline,
      year: earliestBoard.year,
      phase: earliestBoard.phase,
      region: 'centre',
    });
    const endCoordinates = getCoordinates({
      timeline: latestBoard.timeline,
      year: latestBoard.year,
      phase: latestBoard.phase,
      region: 'centre',
    });

    const endOffset = {
      [Phase.Spring]: boardSeparation + majorBoardWidth,
      [Phase.Fall]: boardSeparation + (majorBoardWidth + minorBoardWidth) / 2,
      [Phase.Winter]: boardSeparation + (majorBoardWidth + minorBoardWidth) / 2,
    }[latestBoard.phase];

    return (
      <BoardArrow
        key={timeline}
        startX={startCoordinates.x}
        endX={endCoordinates.x + endOffset}
        startY={startCoordinates.y}
        endY={endCoordinates.y}
        includeHead
      />
    );
  });

  const curvedArrows = world.boards.flatMap((board) =>
    board.childTimelines.map((timeline) => {
      const startBoard = board;
      const endBoard = { ...getNextPhase(board.year, board.phase), timeline };

      const startCoordinates = getCoordinates({
        timeline: startBoard.timeline,
        year: startBoard.year,
        phase: startBoard.phase,
        region: 'centre',
      });
      const endCoordinates = getCoordinates({
        timeline,
        year: endBoard.year,
        phase: endBoard.phase,
        region: 'centre',
      });

      const startOffset = {
        [Phase.Spring]: majorBoardWidth / 2,
        [Phase.Fall]: majorBoardWidth / 2,
        [Phase.Winter]: minorBoardWidth / 2,
      }[startBoard.phase];

      const endOffset = {
        [Phase.Spring]: -majorBoardWidth / 2,
        [Phase.Fall]: -majorBoardWidth / 2,
        [Phase.Winter]: -minorBoardWidth / 2,
      }[endBoard.phase];

      return (
        <div key={timeline}>
          <BoardArrow
            startX={startCoordinates.x + startOffset}
            endX={endCoordinates.x + endOffset}
            startY={startCoordinates.y}
            endY={endCoordinates.y}
          />
          <ParentIndicator
            location={endBoard}
            parentTimeline={board.timeline}
            x={endCoordinates.x}
            y={endCoordinates.y}
          />
        </div>
      );
    }),
  );

  return (
    <>
      {horizontalArrows}
      {curvedArrows}
    </>
  );
};

export default BoardArrowLayer;
