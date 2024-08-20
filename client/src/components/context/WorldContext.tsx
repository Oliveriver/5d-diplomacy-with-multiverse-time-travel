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
import useSubmitOrders from '../../hooks/api/useSubmitOrders';
import { refetchInterval } from '../../utils/constants';
import GameContext from './GameContext';
import Nation from '../../types/enums/nation';

type WorldContextState = {
  world: World | null;
  submitOrders: (orders: Order[]) => Promise<void>;
  isLoading: boolean;
  error: Error | null;
  retry: () => unknown;
};

const initialWorldContextState: WorldContextState = {
  world: null,
  submitOrders: () => Promise.resolve(),
  isLoading: true,
  error: null,
  retry: () => {},
};

const WorldContext = createContext(initialWorldContextState);

export const WorldContextProvider = ({ children }: PropsWithChildren) => {
  const { game } = useContext(GameContext);

  const { world, isLoading, error: worldError, refetch } = useGetWorld();
  const { submitOrders, isPending: isSubmitting, error: submissionError } = useSubmitOrders();

  const [isRefetching, setIsRefetching] = useState(false);

  const isWaitingForAdjudication =
    world?.orders.some((order) => order.status === OrderStatus.New) ?? false;

  const refetchUntilUpdate = useCallback(async () => {
    setIsRefetching(true);
    const currentIteration = world?.iteration;
    const refetched = await refetch();
    if (refetched.error || currentIteration !== refetched.data?.iteration) {
      setIsRefetching(false);
      return refetched;
    }

    await new Promise((resolve) => {
      setTimeout(resolve, refetchInterval);
    });

    return refetchUntilUpdate();
  }, [world?.iteration, refetch]);

  const contextValue = useMemo(
    () => ({
      world,
      submitOrders: async (orders: Order[]) => {
        if (!game || !world) return;
        const players = game.player ? [game.player] : Object.values(Nation);
        await submitOrders({ gameId: game.id, players, orders });
        await refetchUntilUpdate();
      },
      isLoading: isLoading || isSubmitting || isRefetching || isWaitingForAdjudication,
      error: worldError || submissionError,
      retry: () => refetchUntilUpdate(),
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
      refetchUntilUpdate,
    ],
  );

  return <WorldContext.Provider value={contextValue}>{children}</WorldContext.Provider>;
};

export default WorldContext;
