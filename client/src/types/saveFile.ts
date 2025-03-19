import Order from './order';

type SaveFile = {
  hasStrictAdjacencies: boolean;
  iteration: number;
  orders: Order[];
};

export default SaveFile;
