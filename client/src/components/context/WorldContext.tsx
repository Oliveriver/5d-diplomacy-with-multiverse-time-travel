import { createContext, PropsWithChildren, useContext, useEffect, useMemo, useState } from 'react';
import World from '../../types/world';
import Order, { OrderStatus } from '../../types/order';
import useSubmitOrders from '../../hooks/api/useSubmitOrders';
import GameContext from './GameContext';
import Nation from '../../types/enums/nation';
import useGetWorldWebSockets from '../../hooks/api/useGetWorldWebSockets';

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

  const { world, error: worldError, isConnecting, retry } = useGetWorldWebSockets();
  const { submitOrders, isPending: isSubmitting, error: submissionError } = useSubmitOrders();

  const [isWaitingForUpdate, setIsWaitingForUpdate] = useState(false);

  useEffect(() => {
    setIsWaitingForUpdate(false);
  }, [world]);

  const isWaitingForAdjudication =
    world?.orders.some((order) => order.status === OrderStatus.New) ?? false;

  const contextValue = useMemo(
    () => ({
      world,
      submitOrders: async (orders: Order[]) => {
        if (!game || !world) return;
        const players = game.player ? [game.player] : Object.values(Nation);
        setIsWaitingForUpdate(true);
        await submitOrders({ gameId: game.id, players, orders });
      },
      isLoading: isSubmitting || isConnecting || isWaitingForAdjudication || isWaitingForUpdate,
      error: worldError || submissionError,
      retry,
    }),
    [
      game,
      world,
      submitOrders,
      isSubmitting,
      isWaitingForAdjudication,
      isWaitingForUpdate,
      isConnecting,
      worldError,
      submissionError,
      retry,
    ],
  );

  return <WorldContext.Provider value={contextValue}>{children}</WorldContext.Provider>;
};

export default WorldContext;
