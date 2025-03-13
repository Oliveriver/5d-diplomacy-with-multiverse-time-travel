import { createContext, PropsWithChildren, useMemo } from 'react';
import Nation from '../../types/enums/nation';
import useCreateGame from '../../hooks/api/useCreateGame';
import useJoinGame from '../../hooks/api/useJoinGame';
import queryClient from '../../api/queryClient';
import GameState from '../../types/context/gameState';
import useGetPlayersSubmitted from '../../hooks/api/useGetPlayersSubmitted';

const initialGameState: GameState = {
  game: null,
  createGame: () => Promise.resolve(),
  joinGame: () => Promise.resolve(),
  exitGame: () => {},
  isLoading: false,
  error: null,
  playersSubmitted: [],
};

const GameContext = createContext(initialGameState);

export const GameContextProvider = ({ children }: PropsWithChildren) => {
  const {
    createGame,
    game: created,
    isPending: isCreating,
    error: createError,
    reset: resetCreate,
  } = useCreateGame();
  const {
    joinGame,
    game: joined,
    isPending: isJoining,
    error: joinError,
    reset: resetJoin,
  } = useJoinGame();

  const game = created ?? joined;

  const { playersSubmitted } = useGetPlayersSubmitted(game);

  const contextValue = useMemo(
    () => ({
      game,
      createGame: async (
        isSandbox: boolean,
        player: Nation | null,
        hasStrictAdjacencies: boolean,
      ) => createGame({ isSandbox, player, hasStrictAdjacencies }),
      joinGame: async (gameId: number, isSandbox: boolean, player: Nation | null) =>
        joinGame({ gameId, isSandbox, player }),
      exitGame: () => {
        queryClient.removeQueries();
        resetCreate();
        resetJoin();
      },
      isLoading: isCreating || isJoining,
      error: createError ?? joinError,
      playersSubmitted,
    }),
    [
      game,
      createGame,
      joinGame,
      resetCreate,
      resetJoin,
      isCreating,
      isJoining,
      createError,
      joinError,
      playersSubmitted,
    ],
  );

  return <GameContext.Provider value={contextValue}>{children}</GameContext.Provider>;
};

export default GameContext;
