import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useMemo,
  useState,
} from 'react';
import World from '../../types/world';
import Order, { OrderStatus } from '../../types/order';
import useGetWorld from '../../hooks/api/useGetWorld';
import useGetIteration from '../../hooks/api/useGetIteration';
import useSubmitOrders from '../../hooks/api/useSubmitOrders';
import { iterationRefetchInterval } from '../../utils/constants';
import GameContext from './GameContext';
import Nation from '../../types/enums/nation';
import Board, { getActiveBoards } from '../../types/board';
import queryClient from '../../api/queryClient';

type BoardState = {
  activeBoards: Board[];
  isRetreatTurn: boolean;
};

type WorldContextState = {
  world: World | null;
  submitOrders: (orders: Order[]) => Promise<void>;
  isLoading: boolean;
  error: Error | null;
  retry: () => unknown;
  boardState: BoardState | null;
};

const initialWorldContextState: WorldContextState = {
  world: null,
  submitOrders: () => Promise.resolve(),
  isLoading: true,
  error: null,
  retry: () => {},
  boardState: null,
};

const WorldContext = createContext(initialWorldContextState);

export const WorldContextProvider = ({ children }: PropsWithChildren) => {
  const { game } = useContext(GameContext);

  const { world, isLoading, error: worldError, refetch: refetchWorld } = useGetWorld();
  const { error: iterationError, refetch: refetchIteration } = useGetIteration();
  const { submitOrders, isPending: isSubmitting, error: submissionError } = useSubmitOrders();

  const [isRefetching, setIsRefetching] = useState(false);

  const isWaitingForAdjudication =
    world?.orders.some((order) => order.status === OrderStatus.New) ?? false;

  const refetchUntilUpdate = useCallback(async () => {
    setIsRefetching(true);
    queryClient.refetchQueries({ queryKey: ['getPlayersSubmitted', game?.id] });

    const currentIteration = world?.iteration;
    const refetched = await refetchIteration();
    if (refetched.error || currentIteration !== refetched.data) {
      queryClient.removeQueries({ queryKey: ['getPlayersSubmitted', game?.id] });

      setIsRefetching(false);
      const worldQuery = await refetchWorld();
      return worldQuery.data;
    }

    await new Promise((resolve) => {
      setTimeout(resolve, iterationRefetchInterval);
    });

    return refetchUntilUpdate();
  }, [game?.id, world?.iteration, refetchIteration, refetchWorld]);

  const contextValue = useMemo<WorldContextState>(
    () => ({
      world,
      submitOrders: async (orders: Order[]) => {
        if (!game || !world) return;
        const players = game.player ? [game.player] : Object.values(Nation);
        await submitOrders({ gameId: game.id, players, orders });
        await refetchUntilUpdate();
      },
      isLoading: isLoading || isSubmitting || isRefetching || isWaitingForAdjudication,
      error: worldError || submissionError || iterationError,
      retry: () => refetchUntilUpdate(),
      boardState: world && {
        activeBoards: getActiveBoards(world.boards),
        isRetreatTurn: world.boards.some((board) =>
          Object.values(board.units).some((unit) => unit.mustRetreat),
        ),
      },
    }),
    [
      game,
      world,
      submitOrders,
      isLoading,
      isSubmitting,
      isRefetching,
      isWaitingForAdjudication,
      worldError,
      submissionError,
      iterationError,
      refetchUntilUpdate,
    ],
  );

  return <WorldContext.Provider value={contextValue}>{children}</WorldContext.Provider>;
};

export default WorldContext;
