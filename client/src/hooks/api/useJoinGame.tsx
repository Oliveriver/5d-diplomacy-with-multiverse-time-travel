import axios from 'axios';
import { useMutation } from '@tanstack/react-query';
import Nation from '../../types/enums/nation';
import Game from '../../types/game';
import routes from '../../api/routes';

type JoinRequest = {
  gameId: number;
  player: Nation | null;
};

const joinGame = async ({ gameId, player }: JoinRequest): Promise<Game> => {
  const route = routes.joinGame(gameId);
  const response = await axios.put<Game>(route, {
    player,
  });
  return response.data;
};

const useJoinGame = () => {
  const { mutate, data, ...rest } = useMutation({
    mutationKey: ['joinGame'],
    mutationFn: joinGame,
  });
  return { joinGame: mutate, game: data ?? null, ...rest };
};

export default useJoinGame;
