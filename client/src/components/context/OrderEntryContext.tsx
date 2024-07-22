import { PropsWithChildren, createContext, useEffect } from 'react';
import InputMode from '../../types/enums/inputMode';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';
import useOrderEntryReducer, { initialOrderEntryState } from '../../hooks/useOrderEntryReducer';

const OrderEntryContext = createContext(initialOrderEntryState);

export const OrderEntryContextProvider = ({ children }: PropsWithChildren) => {
  const contextValue = useOrderEntryReducer();
  const { dispatch } = contextValue;

  useEffect(() => {
    const handleKeyPress = (event: KeyboardEvent) => {
      switch (event.key) {
        case '1':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Hold });
          break;
        case '2':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Move });
          break;
        case '3':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Support });
          break;
        case '4':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Convoy });
          break;
        case '5':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Build });
          break;
        case '6':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.Disband });
          break;
        case 'Escape':
          dispatch({ $type: OrderEntryActionType.SetMode, mode: InputMode.None });
          break;
        default:
          break;
      }
    };

    document.addEventListener('keydown', handleKeyPress, true);
    return document.removeEventListener('keydown', handleKeyPress);
  }, [dispatch]);

  return <OrderEntryContext.Provider value={contextValue}>{children}</OrderEntryContext.Provider>;
};

export default OrderEntryContext;
