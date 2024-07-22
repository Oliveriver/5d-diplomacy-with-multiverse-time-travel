import InputMode from '../enums/inputMode';
import Unit from '../unit';
import Location from '../location';
import Nation from '../enums/nation';

export enum OrderEntryActionType {
  LoadWorld = 'LoadWorld',
  Add = 'Add',
  Remove = 'Remove',
  SetMode = 'SetMode',
  SetAvailableModes = 'SetAvailableModes',
  Submit = 'Submit',
  HighlightStart = 'HighlightStart',
  HighlightStop = 'HighlightStop',
}

export type LoadWorldAction = {
  $type: OrderEntryActionType.LoadWorld;
  player: Nation | null;
};

export type AddOrderAction = {
  $type: OrderEntryActionType.Add;
  unit?: Unit;
  location: Location;
};

export type RemoveOrderAction = {
  $type: OrderEntryActionType.Remove;
  location: Location;
};

export type SetInputModeAction = {
  $type: OrderEntryActionType.SetMode;
  mode: InputMode;
};

export type SetAvailableModesAction = {
  $type: OrderEntryActionType.SetAvailableModes;
  modes: InputMode[];
};

export type SubmitAction = {
  $type: OrderEntryActionType.Submit;
};

export type HighlightStartAction = {
  $type: OrderEntryActionType.HighlightStart;
  location: Location;
};

export type HighlightStopAction = {
  $type: OrderEntryActionType.HighlightStop;
};

type OrderEntryAction =
  | SetInputModeAction
  | SetAvailableModesAction
  | SubmitAction
  | AddOrderAction
  | RemoveOrderAction
  | LoadWorldAction
  | HighlightStartAction
  | HighlightStopAction;

export default OrderEntryAction;
