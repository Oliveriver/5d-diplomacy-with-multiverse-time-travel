import { TransformComponent, TransformWrapper } from 'react-zoom-pan-pinch';
import { useContext } from 'react';
import BoardLayer from './world/boards/BoardLayer';
import { initialScale } from '../utils/constants';
import Overlay from './user-interface/Overlay';
import { OrderEntryContextProvider } from './context/OrderEntryContext';
import OrderLayer from './world/orders/OrderLayer';
import WorldLoading from './world/WorldLoading';
import WorldError from './world/WorldError';
import BoardArrowLayer from './world/arrows/BoardArrowLayer';
import WorldContext from './context/WorldContext';
import { getDefaultOffsetX, getDefaultOffsetY } from '../utils/navigationUtils';

const GameRoot = () => {
  const { world, isLoading, error, retry } = useContext(WorldContext);

  return (
    <OrderEntryContextProvider>
      <TransformWrapper
        limitToBounds={false}
        minScale={0.05}
        maxScale={3}
        smooth
        wheel={{ smoothStep: 0.0005 }}
        initialScale={initialScale}
        initialPositionX={getDefaultOffsetX()}
        initialPositionY={getDefaultOffsetY()}
        doubleClick={{ disabled: true }}
        panning={{ excluded: ['input', 'select'], velocityDisabled: true }}
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
