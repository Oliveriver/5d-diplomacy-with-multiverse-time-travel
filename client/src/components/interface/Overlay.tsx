import SubmitButton from './SubmitButton';
import OrderList from './OrderList';
import VictoryNotice from './VictoryNotice';
import InputModeButtonList from './InputModeButtonList';
import PlayerList from './PlayerList';
import ExitButton from './ExitButton';

const Overlay = () => (
  <>
    <PlayerList />
    <VictoryNotice />
    <InputModeButtonList />
    <SubmitButton />
    <OrderList />
    <ExitButton />
  </>
);

export default Overlay;
