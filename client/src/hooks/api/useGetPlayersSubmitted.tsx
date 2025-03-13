import axios from 'axios';
import { useQuery } from '@tanstack/react-query';
import routes from '../../api/routes';
import Nation from '../../types/enums/nation';
import Game from '../../types/game';
import { playersSubmittedRefetchInterval } from '../../utils/constants';

const getPlayersSubmitted = async (game: Game | null): Promise<Nation[] | null> => {
  if (!game) return null;

  const { id } = game;

  const route = routes.getPlayersSubmitted(id);
  const response = await axios.get<Nation[]>(route);
  return response.data;
};

const useGetPlayersSubmitted = (game: Game | null) => {
  const { data, ...rest } = useQuery({
    queryKey: ['getPlayersSubmitted', game?.id],
    queryFn: () => getPlayersSubmitted(game),
    refetchInterval: playersSubmittedRefetchInterval,
  });

  return { playersSubmitted: data ?? [], ...rest };
};

export default useGetPlayersSubmitted;
