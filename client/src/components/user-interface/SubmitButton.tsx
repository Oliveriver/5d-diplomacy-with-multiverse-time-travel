import { useContext } from 'react';
import Button from './common/Button';
import OrderEntryContext from '../context/OrderEntryContext';
import { OrderEntryActionType } from '../../types/context/orderEntryAction';
import WorldContext from '../context/WorldContext';

const SubmitButton = () => {
  const { world, submitOrders, isLoading, error } = useContext(WorldContext);
  const { dispatch, orders } = useContext(OrderEntryContext);

  const onSubmit = () => {
    dispatch({ $type: OrderEntryActionType.Submit });
    submitOrders(orders);
  };

  return (
    <div className="absolute right-10 bottom-10">
      <Button
        text="Submit"
        onClick={onSubmit}
        minWidth={200}
        isDisabled={isLoading || error !== null}
        isBusy={world !== null && !error && isLoading}
      />
    </div>
  );
};

export default SubmitButton;
