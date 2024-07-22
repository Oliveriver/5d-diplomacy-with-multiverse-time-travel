import Unit, { displayUnit } from './unit';
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
  Retreat = 'Retreat',
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
    [OrderStatus.Retreat]: colours.orderRetreat,
  }[typeof order === 'object' ? order.status : order];
};

export const displayOrder = (order: Order) => {
  const unit = displayUnit(order?.unit);
  const location = displayLocation(order.location);

  switch (order.$type) {
    case OrderType.Move:
      return `${unit} ${location} => ${displayLocation(order.destination)}`;
    case OrderType.Support:
      return `${unit} ${location} S ${displayLocation(order.supportLocation)} => ${displayLocation(order.destination)}`;
    case OrderType.Convoy:
      return `${unit} ${location} C ${displayLocation(order.convoyLocation)} => ${displayLocation(order.destination)}`;
    case OrderType.Build:
      return `+ ${unit} ${location}`;
    case OrderType.Disband:
      return `- ${unit} ${location}`;
    default:
      return `${unit} ${location} H`;
  }
};

export default Order;
