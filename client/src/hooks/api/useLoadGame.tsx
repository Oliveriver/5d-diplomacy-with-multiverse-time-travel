import { useMutation } from '@tanstack/react-query';
import axios from 'axios';
import Game from '../../types/game';
import Nation from '../../types/enums/nation';
import routes from '../../api/routes';

type LoadRequest = {
  isSandbox: boolean;
  player: Nation | null;
  file: File;
};

const loadGame = async ({ isSandbox, player, file }: LoadRequest): Promise<Game> => {
  const route = routes.loadGame();
  const formData = new FormData();
  formData.append('isSandbox', isSandbox.toString());
  formData.append('player', player?.toString() ?? '');
  formData.append('file', file);
  const config = {
    headers: {
      'content-type': 'multipart/form-data',
    },
  };
  const response = await axios.post<Game>(route, formData, config);
  return response.data;
};

const useLoadGame = () => {
  const { mutate, data, ...rest } = useMutation({
    mutationKey: ['loadGame'],
    mutationFn: loadGame,
  });
  return { loadGame: mutate, game: data ?? null, ...rest };
};

export default useLoadGame;
