import { useQuery } from '@tanstack/react-query';
import { useContext } from 'react';
import axios from 'axios';
import Game from '../../types/game';
import GameContext from '../../components/context/GameContext';
import routes from '../../api/routes';

const getIteration = async (game: Game | null): Promise<number | null> => {
  if (!game) return null;

  const { id, player } = game;

  const route = routes.getIteration(id);
  const response = await axios.get<number>(route, {
    params: { player },
  });
  return response.data;
};

const useGetIteration = () => {
  const { game } = useContext(GameContext);

  const { data, ...rest } = useQuery({
    queryKey: ['getIteration', game?.id, game?.player],
    queryFn: () => getIteration(game),
  });

  return { iteration: data ?? null, ...rest };
};

export default useGetIteration;
