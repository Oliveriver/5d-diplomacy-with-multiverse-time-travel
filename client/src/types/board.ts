import Nation from './enums/nation';
import Unit from './unit';
import Location, { getLocationKey } from './location';

type Board = Omit<Location, 'region'> & {
  childTimelines: number[];
  centres: { [id: string]: Nation };
  units: { [location: string]: Unit };
};

export const getBoardKey = (board: Board | Omit<Location, 'region'>) => {
  const { timeline, year, phase } = board;
  return getLocationKey({ timeline, year, phase, region: '' });
};

export default Board;
