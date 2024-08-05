import { TransformComponent, TransformWrapper } from 'react-zoom-pan-pinch';
import { useContext } from 'react';
import BoardLayer from './world/boards/BoardLayer';
import { boardSeparation, initialScale, majorBoardWidth } from '../utils/constants';
import Overlay from './user-interface/Overlay';
import { OrderEntryContextProvider } from './context/OrderEntryContext';
import OrderLayer from './world/orders/OrderLayer';
import WorldLoading from './world/WorldLoading';
import WorldError from './world/WorldError';
import BoardArrowLayer from './world/arrows/BoardArrowLayer';
import WorldContext from './context/WorldContext';

const GameRoot = () => {
  const { world, isLoading, error, retry } = useContext(WorldContext);

  const initialOffsetX =
    (window.innerWidth - initialScale * (majorBoardWidth + boardSeparation)) / 2;
  const initialOffsetY =
    (window.innerHeight - initialScale * (majorBoardWidth + boardSeparation)) / 2 - 40;

  return (
    <OrderEntryContextProvider>
      <TransformWrapper
        limitToBounds={false}
        minScale={0.05}
        maxScale={3}
        initialScale={initialScale}
        initialPositionX={initialOffsetX}
        initialPositionY={initialOffsetY}
        doubleClick={{ disabled: true }}
        panning={{ excluded: ['input', 'select'] }}
      >
        {error && <WorldError error={error} retry={retry} isLoading={isLoading} />}
        {!world && <WorldLoading />}
        {!error && (
          <TransformComponent>
            <BoardArrowLayer />
            <BoardLayer />
            <OrderLayer />
          </TransformComponent>
        )}
        <Overlay />
      </TransformWrapper>
    </OrderEntryContextProvider>
  );
};

export default GameRoot;
