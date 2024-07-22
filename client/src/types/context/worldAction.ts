import World from '../world';

export enum WorldActionType {
  SetLoading = 'SetLoading',
  SetLoaded = 'SetLoaded',
  SetError = 'SetError',
}

type WorldAction = {
  $type: WorldActionType;
  world?: World;
  error?: Error;
};

export default WorldAction;
