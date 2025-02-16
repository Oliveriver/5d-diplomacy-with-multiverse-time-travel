import { useContext, useMemo, useState } from 'react';
import useWebSocket from 'react-use-websocket';
import GameContext from '../../components/context/GameContext';
import routes from '../../api/routes';
import World from '../../types/world';

type WebSocketState = {
  error: Error | null;
  connect: boolean;
  isConnecting: boolean;
};

const useGetWorldWebSockets = () => {
  const { game } = useContext(GameContext);

  const [wsState, setWsState] = useState<WebSocketState>({
    error: null,
    connect: true,
    isConnecting: true,
  });

  const url = useMemo(() => (game ? routes.getWorldWebSocket(game.id) : ''), [game]);
  const queryParams = useMemo(
    () => (game?.player ? { player: `${game.player}` } : undefined),
    [game],
  );

  const { lastJsonMessage } = useWebSocket<World>(
    url,
    {
      onOpen: () => {
        setWsState((prev) => ({ ...prev, isConnecting: false }));
      },
      onClose: () => {
        setWsState({
          connect: false,
          isConnecting: false,
          error: Error('WebSocket failed to connect to backend server'),
        });
      },
      queryParams,
      heartbeat: {
        timeout: 30000,
        interval: 5000,
        returnMessage: 'pong',
      },
    },
    wsState.connect,
  );

  const retry = () => {
    setWsState({ connect: true, isConnecting: true, error: null });
  };

  return {
    world: lastJsonMessage,
    error: wsState.error,
    retry,
    isConnecting: wsState.isConnecting,
  };
};

export default useGetWorldWebSockets;
