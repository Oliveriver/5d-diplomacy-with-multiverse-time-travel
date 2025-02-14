import { createContext, PropsWithChildren, useContext, useMemo, useState } from 'react';
import World from '../../types/world';
import Order, { OrderStatus } from '../../types/order';
import useGetWorld from '../../hooks/api/useGetWorld';
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

  const { world, isLoading, error: worldError, refetch } = useGetWorld();
  const { lastMessage } = useGetWorldWebSockets();
  const { submitOrders, isPending: isSubmitting, error: submissionError } = useSubmitOrders();

  const [isWaitingForUpdate, setIsWaitingForUpdate] = useState(false);

  const websocketWorld = useMemo(() => {
    setIsWaitingForUpdate(false);
    return lastMessage ? (JSON.parse(lastMessage.data) as World) : null;
  }, [lastMessage]);

  const latestWorld =
    websocketWorld && (!world || websocketWorld.iteration > world.iteration)
      ? websocketWorld
      : world;

  const isWaitingForAdjudication =
    latestWorld?.orders.some((order) => order.status === OrderStatus.New) ?? false;

  const contextValue = useMemo(
    () => ({
      world: latestWorld,
      submitOrders: async (orders: Order[]) => {
        if (!game || !latestWorld) return;
        const players = game.player ? [game.player] : Object.values(Nation);
        setIsWaitingForUpdate(true);
        await submitOrders({ gameId: game.id, players, orders });
      },
      isLoading: isLoading || isSubmitting || isWaitingForAdjudication || isWaitingForUpdate,
      error: worldError || submissionError,
      retry: () => refetch(),
    }),
    [
      game,
      latestWorld,
      submitOrders,
      isLoading,
      isSubmitting,
      isWaitingForAdjudication,
      isWaitingForUpdate,
      worldError,
      submissionError,
      refetch,
    ],
  );

  return <WorldContext.Provider value={contextValue}>{children}</WorldContext.Provider>;
};

export default WorldContext;
