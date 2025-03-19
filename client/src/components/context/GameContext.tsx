import { createContext, PropsWithChildren, useMemo } from 'react';
import Nation from '../../types/enums/nation';
import useCreateGame from '../../hooks/api/useCreateGame';
import useJoinGame from '../../hooks/api/useJoinGame';
import useLoadGame from '../../hooks/api/useLoadGame';
import queryClient from '../../api/queryClient';
import GameState from '../../types/context/gameState';
import useGetPlayersSubmitted from '../../hooks/api/useGetPlayersSubmitted';

const initialGameState: GameState = {
  game: null,
  createGame: () => Promise.resolve(),
  joinGame: () => Promise.resolve(),
  loadGame: () => Promise.resolve(),
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
  const {
    loadGame,
    game: loaded,
    isPending: isLoading,
    error: loadError,
    reset: resetLoad,
  } = useLoadGame();

  const game = created ?? joined ?? loaded;

  const { playersSubmitted } = useGetPlayersSubmitted(game);

  const contextValue = useMemo<GameState>(
    () => ({
      game,
      createGame: async (
        isSandbox: boolean,
        player: Nation | null,
        hasStrictAdjacencies: boolean,
      ) => createGame({ isSandbox, player, hasStrictAdjacencies }),
      joinGame: async (gameId: number, isSandbox: boolean, player: Nation | null) =>
        joinGame({ gameId, isSandbox, player }),
      loadGame: async (isSandbox: boolean, player: Nation | null, file: File) =>
        loadGame({ isSandbox, player, file }),
      exitGame: () => {
        queryClient.removeQueries();
        resetCreate();
        resetJoin();
        resetLoad();
      },
      isLoading: isCreating || isJoining || isLoading,
      error: createError ?? joinError ?? loadError,
      playersSubmitted,
    }),
    [
      game,
      createGame,
      joinGame,
      loadGame,
      resetCreate,
      resetJoin,
      resetLoad,
      isCreating,
      isJoining,
      isLoading,
      createError,
      joinError,
      loadError,
      playersSubmitted,
    ],
  );

  return <GameContext.Provider value={contextValue}>{children}</GameContext.Provider>;
};

export default GameContext;
