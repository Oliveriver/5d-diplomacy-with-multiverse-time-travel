import { useContext, useMemo, useState } from 'react';
import useWebSocket from 'react-use-websocket';
import GameContext from '../../components/context/GameContext';
import routes from '../../api/routes';
import World from '../../types/world';
import { webSocketPingInterval, webSocketTimeout } from '../../utils/constants';

type WebSocketState = {
  error: Error | null;
  connect: boolean;
  isConnecting: boolean;
};

const useGetWorldWebSockets = () => {
  const { game } = useContext(GameContext);

  const [webSocketState, setWebSocketState] = useState<WebSocketState>({
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
        setWebSocketState((prev) => ({ ...prev, isConnecting: false }));
      },
      onClose: () => {
        setWebSocketState({
          connect: false,
          isConnecting: false,
          error: Error('WebSocket failed to connect to backend server'),
        });
      },
      queryParams,
      heartbeat: {
        timeout: webSocketTimeout,
        interval: webSocketPingInterval,
        returnMessage: 'pong',
      },
    },
    webSocketState.connect,
  );

  const retry = () => {
    setWebSocketState({ connect: true, isConnecting: true, error: null });
  };

  return {
    world: lastJsonMessage,
    error: webSocketState.error,
    retry,
    isConnecting: webSocketState.isConnecting,
  };
};

export default useGetWorldWebSockets;
