import Order from './order';
import Board, { compareBoards } from './board';
import Nation from './enums/nation';
import Location from './location';

type World = {
  iteration: number;
  boards: Board[];
  orders: Order[];
  winner: Nation | null;
};

export const findUnit = (world: World | null, location: Location) =>
  world?.boards.find((board) => compareBoards(board, location))?.units[location.region] ?? null;

export default World;
