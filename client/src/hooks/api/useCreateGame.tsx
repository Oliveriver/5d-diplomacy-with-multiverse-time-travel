import { useMutation } from '@tanstack/react-query';
import axios from 'axios';
import Game from '../../types/game';
import Nation from '../../types/enums/nation';
import routes from '../../api/routes';

type CreationRequest = {
  isSandbox: boolean;
  player: Nation | null;
  hasStrictAdjacencies: boolean;
};

const createGame = async ({
  isSandbox,
  player,
  hasStrictAdjacencies,
}: CreationRequest): Promise<Game> => {
  const route = routes.createGame();
  const response = await axios.post<Game>(route, {
    isSandbox,
    player,
    hasStrictAdjacencies,
  });
  return response.data;
};

const useCreateGame = () => {
  const { mutate, data, ...rest } = useMutation({
    mutationKey: ['createGame'],
    mutationFn: createGame,
  });
  return { createGame: mutate, game: data ?? null, ...rest };
};

export default useCreateGame;
