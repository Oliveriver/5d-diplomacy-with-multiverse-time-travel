import { useTransformContext } from 'react-zoom-pan-pinch';
import { useEffect, useState } from 'react';
import { initialScale } from '../../utils/constants';
import { getDefaultOffsetX, getDefaultOffsetY } from '../../utils/navigationUtils';

const CoordinateDisplay = () => {
  const { setTransformState, onChangeCallbacks } = useTransformContext();

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

  const coordinateText = `${scale} (${positionX}, ${positionY})`;

  const resetTransform = () =>
    setTransformState(initialScale, getDefaultOffsetX(), getDefaultOffsetY());

  return (
    <button
      type="button"
      className="absolute top-3 right-10 text-xs font-mono opacity-50"
      onClick={resetTransform}
      title="Reset view"
    >
      {coordinateText}
    </button>
  );
};

export default CoordinateDisplay;
