import Order from './order';
import Board from './board';
import Nation from './enums/nation';

type World = {
  iteration: number;
  boards: Board[];
  orders: Order[];
  winner: Nation | null;
};

export default World;
