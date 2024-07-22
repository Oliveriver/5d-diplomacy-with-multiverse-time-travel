const baseUrl = import.meta.env.VITE_SERVER_URL;

const routes = {
  createGame: () => `${baseUrl}/game`,
  joinGame: (gameId: number) => `${baseUrl}/game/${gameId}/players`,
  getWorld: (gameId: number) => `${baseUrl}/game/${gameId}`,
  submitOrders: (gameId: number) => `${baseUrl}/game/${gameId}/orders`,
};

export default routes;
