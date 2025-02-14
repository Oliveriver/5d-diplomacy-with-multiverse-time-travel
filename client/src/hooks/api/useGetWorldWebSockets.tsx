import { useContext } from 'react';
import useWebSocket from 'react-use-websocket';
import { QueryParams } from 'react-use-websocket/dist/lib/types';
import GameContext from '../../components/context/GameContext';
import routes from '../../api/routes';

const useGetWorldWebSockets = () => {
  const { game } = useContext(GameContext);

  const queryParams: QueryParams | undefined = game?.player
    ? { player: `${game.player}` }
    : undefined;

  const { lastMessage } = useWebSocket(game ? routes.getWorldWebSocket(game.id) : '', {
    retryOnError: true,
    reconnectAttempts: 10,
    reconnectInterval: (attemptNumber) => Math.min(2 ** attemptNumber * 1000, 10000),
    queryParams,
  });

  return { lastMessage };
};

export default useGetWorldWebSockets;
