import { useMutation } from '@tanstack/react-query';
import axios from 'axios';
import Order from '../../types/order';
import routes from '../../api/routes';
import Nation from '../../types/enums/nation';

type SubmissionRequest = {
  gameId: number;
  players: Nation[];
  orders: Order[];
};

const submitOrders = async ({ gameId, players, orders }: SubmissionRequest) => {
  const route = routes.submitOrders(gameId);
  const response = await axios.post(route, {
    players,
    orders,
  });
  return response.data;
};

const useSubmitOrders = () => {
  const { mutateAsync, ...rest } = useMutation({
    mutationKey: ['submitOrders'],
    mutationFn: submitOrders,
  });
  return { submitOrders: mutateAsync, ...rest };
};

export default useSubmitOrders;
