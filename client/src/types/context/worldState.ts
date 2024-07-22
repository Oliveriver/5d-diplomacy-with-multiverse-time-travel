import World from '../world';
import WorldAction from './worldAction';

type WorldState = {
  isLoading: boolean;
  error: Error | null;
  world: World;
  dispatch: (action: WorldAction) => void;
  refetch: () => Promise<unknown>;
};

export default WorldState;
