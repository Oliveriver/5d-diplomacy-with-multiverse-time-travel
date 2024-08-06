import Nation from './enums/nation';

type Game = {
  id: number;
  hasStrictAdjacencies: boolean;
  player: Nation | null;
};

export default Game;
