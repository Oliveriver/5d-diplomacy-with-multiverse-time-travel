import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useMemo,
  useState,
} from 'react';
import World from '../../types/world';
import Order from '../../types/order';
import useGetWorld from '../../hooks/api/useGetWorld';
import useSubmitOrders from '../../hooks/api/useSubmitOrders';
import { refetchAttempts, refetchInterval } from '../../utils/constants';
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

  const refetchUntilUpdate = useCallback(
    async (attempts: number) => {
      setIsRefetching(true);
      const currentIteration = world?.iteration;
      const refetched = await refetch();
      if (attempts <= 1 || refetched.error || currentIteration !== refetched.data?.iteration) {
        setIsRefetching(false);
        return refetched;
      }

      await new Promise((resolve) => {
        setTimeout(resolve, refetchInterval);
      });

      return refetchUntilUpdate(attempts - 1);
    },
    [world?.iteration, refetch],
  );

  const contextValue = useMemo(
    () => ({
      world,
      submitOrders: async (orders: Order[]) => {
        if (!game || !world) return;
        const players = game.player ? [game.player] : Object.values(Nation);
        submitOrders({ gameId: game.id, players, orders });
        await refetchUntilUpdate(refetchAttempts);
      },
      isLoading: isLoading || isSubmitting || isRefetching,
      error: worldError || submissionError,
      retry: () => refetchUntilUpdate(refetchAttempts),
    }),
    [
      game,
      world,
      submitOrders,
      isLoading,
      isSubmitting,
      isRefetching,
      worldError,
      submissionError,
      refetchUntilUpdate,
    ],
  );

  return <WorldContext.Provider value={contextValue}>{children}</WorldContext.Provider>;
};

export default WorldContext;
