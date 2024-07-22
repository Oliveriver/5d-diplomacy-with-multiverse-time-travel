import InputMode from '../enums/inputMode';
import Nation from '../enums/nation';
import Order from '../order';
import OrderEntryAction from './orderEntryAction';

type OrderEntryState = {
  player: Nation | null;
  orders: Order[];
  currentMode: InputMode;
  currentOrder: Order | null;
  highlightedOrder: Order | null;
  availableModes: InputMode[];
  dispatch: (action: OrderEntryAction) => void;
};

export default OrderEntryState;
