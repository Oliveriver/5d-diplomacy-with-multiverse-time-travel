import { createContext, PropsWithChildren, useEffect, useMemo, useState } from 'react';
import { useTransformContext } from 'react-zoom-pan-pinch';
import { initialScale } from '../../utils/constants';
import { getDefaultOffsetX, getDefaultOffsetY } from '../../utils/navigationUtils';

type ScaleContextState = {
  scale: number;
  positionX: number;
  positionY: number;
};

const initialScaleContextState: ScaleContextState = {
  scale: initialScale,
  positionX: getDefaultOffsetX(),
  positionY: getDefaultOffsetY(),
};

const ScaleContext = createContext(initialScaleContextState);

export const ScaleContextProvider = ({ children }: PropsWithChildren) => {
  const { onChangeCallbacks } = useTransformContext();

  const [scale, setScale] = useState(initialScale);
  const [positionX, setPositionX] = useState(getDefaultOffsetX());
  const [positionY, setPositionY] = useState(getDefaultOffsetY());

  useEffect(() => {
    const callback = ({
      state,
    }: {
      state: { scale: number; positionX: number; positionY: number };
    }) => {
      if (scale !== state.scale || positionX !== state.positionX || positionY !== state.positionY) {
        setScale(state.scale);
        setPositionX(state.positionX);
        setPositionY(state.positionY);
      }
    };

    onChangeCallbacks.add(callback);
    return () => {
      onChangeCallbacks.delete(callback);
    };
  }, [onChangeCallbacks, scale, positionX, positionY]);

  const contextValue: ScaleContextState = useMemo(
    () => ({
      scale,
      positionX,
      positionY,
    }),
    [scale, positionX, positionY],
  );

  return <ScaleContext.Provider value={contextValue}>{children}</ScaleContext.Provider>;
};

export default ScaleContext;
