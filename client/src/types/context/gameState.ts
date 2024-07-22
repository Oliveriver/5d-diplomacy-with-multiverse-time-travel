import Nation from '../enums/nation';
import Game from '../game';

type GameState = {
  game: Game | null;
  createGame: (isSandbox: boolean, player: Nation | null) => Promise<void>;
  joinGame: (gameId: number, player: Nation | null) => Promise<void>;
  exitGame: () => void;
  isLoading: boolean;
  error: Error | null;
};

export default GameState;
