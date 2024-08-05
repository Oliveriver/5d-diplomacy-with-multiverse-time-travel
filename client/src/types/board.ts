import Nation from './enums/nation';
import Unit from './unit';
import Location, { getLocationKey } from './location';
import { filterUnique } from '../utils/listUtils';
import { getLatestPhase } from './enums/phase';

type Board = Omit<Location, 'region'> & {
  childTimelines: number[];
  centres: { [id: string]: Nation };
  units: { [location: string]: Unit };
};

export const getBoardKey = (board: Board | Omit<Location, 'region'>) => {
  const { timeline, year, phase } = board;
  return getLocationKey({ timeline, year, phase, region: '' });
};

export const getBoardName = ({ timeline, year, phase }: Omit<Location, 'region'>) =>
  `Timeline ${timeline} - ${phase} ${year}`;

export const compareBoards = (board1: Omit<Location, 'region'>, board2: Omit<Location, 'region'>) =>
  board1.timeline === board2.timeline &&
  board1.year === board2.year &&
  board1.phase === board2.phase;

export const getActiveBoards = (boards: Board[]) => {
  const timelines = filterUnique(boards.map(({ timeline }) => timeline));
  return timelines.map((timeline) =>
    boards
      .filter((board) => board.timeline === timeline)
      .reduce((board1, board2) => {
        if (board1.year > board2.year) return board1;
        if (board2.year > board1.year) return board2;

        return getLatestPhase(board1.phase, board2.phase) === board1.phase ? board1 : board2;
      }),
  );
};

export default Board;
