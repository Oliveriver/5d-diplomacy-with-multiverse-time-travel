import SubmitButton from './SubmitButton';
import OrderList from './OrderList';
import VictoryNotice from './VictoryNotice';
import InputModeButtonList from './InputModeButtonList';
import PlayerList from './PlayerList';
import ExitButton from './ExitButton';
import CoordinateDisplay from './CoordinateDisplay';

const Overlay = () => (
  <>
    <PlayerList />
    <VictoryNotice />
    <InputModeButtonList />
    <SubmitButton />
    <OrderList />
    <ExitButton />
    <CoordinateDisplay />
  </>
);

export default Overlay;
