import { useTransformContext } from 'react-zoom-pan-pinch';
import { useContext } from 'react';
import { initialScale } from '../../utils/constants';
import { getDefaultOffsetX, getDefaultOffsetY } from '../../utils/navigationUtils';
import ScaleContext from '../context/ScaleContext';

const CoordinateDisplay = () => {
  const { setTransformState } = useTransformContext();
  const { scale, positionX, positionY } = useContext(ScaleContext);

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
