import { useQuery } from '@tanstack/react-query';
import { useContext } from 'react';
import axios from 'axios';
import World from '../../types/world';
import Game from '../../types/game';
import GameContext from '../../components/context/GameContext';
import routes from '../../api/routes';

const getWorld = async (game: Game | null): Promise<World | null> => {
  if (!game) return null;

  const route = routes.getWorld(game.id);
  const response = await axios.get<World>(route);
  return response.data;
};

const useGetWorld = () => {
  const { game } = useContext(GameContext);

  const { data, ...rest } = useQuery({
    queryKey: ['getWorld', game?.id, game?.player],
    queryFn: () => getWorld(game),
  });

  return { world: data ?? null, ...rest };
};

export default useGetWorld;
