import Unit from './unit';
import Location, { displayLocation, getLocationKey } from './location';
import colours from '../utils/colours';

export enum OrderType {
  Hold = 'Hold',
  Move = 'Move',
  Support = 'Support',
  Convoy = 'Convoy',
  Build = 'Build',
  Disband = 'Disband',
}

export enum OrderStatus {
  New = 'New',
  Success = 'Success',
  Failure = 'Failure',
  Invalid = 'Invalid',
  RetreatNew = 'RetreatNew',
  RetreatSuccess = 'RetreatSuccess',
  RetreatFailure = 'RetreatFailure',
  RetreatInvalid = 'RetreatInvalid',
}

type Hold = {
  $type: OrderType.Hold;
  status: OrderStatus;
  unit: Unit;
  location: Location;
};

type Move = {
  $type: OrderType.Move;
  status: OrderStatus;
  unit: Unit;
  location: Location;
  destination: Location | null;
};

type Support = {
  $type: OrderType.Support;
  status: OrderStatus;
  unit: Unit;
  location: Location;
  supportLocation: Location | null;
  destination: Location | null;
};

type Convoy = {
  $type: OrderType.Convoy;
  status: OrderStatus;
  unit: Unit;
  location: Location;
  convoyLocation: Location | null;
  destination: Location | null;
};

type Build = {
  $type: OrderType.Build;
  status: OrderStatus;
  unit: Unit | null;
  location: Location;
};

type Disband = {
  $type: OrderType.Disband;
  status: OrderStatus;
  unit: Unit;
  location: Location;
};

type Order = Hold | Move | Support | Convoy | Build | Disband;

export const getOrderKey = (order: Order) => `${getLocationKey(order.location)} ${order.status}`;

export const getOrderColour = (order: Order | OrderStatus, isHighlighted: boolean = false) => {
  if (isHighlighted) return colours.orderHighlight;

  return {
    [OrderStatus.New]: colours.orderNew,
    [OrderStatus.Success]: colours.orderSuccess,
    [OrderStatus.Failure]: colours.orderFailure,
    [OrderStatus.Invalid]: colours.orderFailure,
    [OrderStatus.RetreatNew]: colours.orderNew,
    [OrderStatus.RetreatSuccess]: colours.orderRetreat,
    [OrderStatus.RetreatFailure]: colours.orderFailure,
    [OrderStatus.RetreatInvalid]: colours.orderFailure,
  }[typeof order === 'object' ? order.status : order];
};

export const getOrderText = (order: Order) => {
  const unit = order.unit?.type;
  const location = displayLocation(order.location);

  switch (order.$type) {
    case OrderType.Move:
      return `MOVE\n${unit} ${location} moves to ${displayLocation(order.destination)}`;
    case OrderType.Support:
      return `SUPPORT\n${unit} ${location} supports ${displayLocation(order.supportLocation)} to ${displayLocation(order.destination)}`;
    case OrderType.Convoy:
      return `CONVOY\n${unit} ${location} convoys ${displayLocation(order.convoyLocation)} to ${displayLocation(order.destination)}`;
    case OrderType.Build:
      return `BUILD\n${unit} ${location}`;
    case OrderType.Disband:
      return `DISBAND\n${unit} ${location}`;
    default:
      return `HOLD\n${unit} ${location}`;
  }
};

export default Order;
