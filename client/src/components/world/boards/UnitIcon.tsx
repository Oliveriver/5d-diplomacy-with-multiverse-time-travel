import { getNationColour } from '../../../types/enums/nation';
import Unit from '../../../types/unit';
import UnitType from '../../../types/enums/unitType';
import { rasteriseFactor, rasteriseScaleThreshold, unitWidth } from '../../../utils/constants';
import ArmyIcon from '../../../assets/icons/ArmyIcon.svg?react';
import FleetIcon from '../../../assets/icons/FleetIcon.svg?react';
import ArmyIconRaw from '../../../assets/icons/ArmyIcon.svg?raw';
import FleetIconRaw from '../../../assets/icons/FleetIcon.svg?raw';

type UnitIconProps = {
  unit: Unit;
  scaleFactor?: number;
  variant?: 'world' | 'overlay';
};

export const rasteriseUnitIcon = (
  imageCache: Map<string, Promise<CanvasImageSource>>,
  { unit, scaleFactor = 1, variant = 'world' }: UnitIconProps,
  left: number,
  bottom: number,
  canvas: HTMLCanvasElement,
  context: CanvasRenderingContext2D,
  onComplete: () => void,
) => {
  const rasterAdjustment = rasteriseScaleThreshold * rasteriseFactor;

  const isWorldVariant = variant === 'world';
  let svg = unit.type === UnitType.Army ? ArmyIconRaw : FleetIconRaw;

  const width = unitWidth * scaleFactor * rasterAdjustment;
  const margin = isWorldVariant ? -width / 2 : 0;

  // When drawing to canvas, CSS won't be applied, we'll need to apply the same effects manually.
  const computedStyle = window.getComputedStyle(canvas);

  let regionColour = getNationColour(unit.owner);
  regionColour = regionColour.replace('var(', '').replace(')', '');
  regionColour = computedStyle.getPropertyValue(regionColour);

  const drawImage = (image: CanvasImageSource) => {
    context.drawImage(
      image,
      left * rasterAdjustment + margin,
      canvas.height - bottom * rasterAdjustment + margin,
      width,
      width,
    );
    onComplete();
  };

  const cacheKey = `uniticon|${unit.type}|${regionColour}`;
  const cachedImage = imageCache.get(cacheKey);
  if (cachedImage !== undefined) {
    cachedImage.then(drawImage);
  } else {
    // Apply the SVG colour attribute.
    svg = svg.replace('"currentColor"', `"${regionColour}"`);

    // Apply CSS rules for fonts.
    const svgDoc = new DOMParser().parseFromString(svg, 'image/svg+xml');
    svgDoc
      .querySelectorAll('text')
      .forEach((e) => e.setAttribute('font-family', 'ui-sans-serif, system-ui, sans-serif'));
    svg = new XMLSerializer().serializeToString(svgDoc);

    // Create a URL we can load as the image source.
    const blob = new Blob([svg], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);
    const image = new Image();
    image.src = url;
    const promise = new Promise<CanvasImageSource>((resolve) => {
      image.onload = async () => {
        URL.revokeObjectURL(url);
        resolve(image);
        drawImage(image);
      };
    });
    imageCache.set(cacheKey, promise);
  }
};

const UnitIcon = ({ unit, scaleFactor = 1, variant = 'world' }: UnitIconProps) => {
  const isWorldVariant = variant === 'world';
  const Svg = unit.type === UnitType.Army ? ArmyIcon : FleetIcon;

  const width = unitWidth * scaleFactor;

  return (
    <>
      <Svg
        className="flex justify-center overflow-visible"
        style={{
          color: getNationColour(unit.owner),
          width,
          height: width,
          margin: isWorldVariant ? -width / 2 : 0,
          zIndex: unit.mustRetreat ? 30 : 10,
          position: isWorldVariant ? 'absolute' : 'relative',
          pointerEvents: isWorldVariant ? 'none' : 'auto',
        }}
      />
      {isWorldVariant && unit.mustRetreat && (
        <div style={{ boxShadow: '0px 0px 35px 35px red', zIndex: 5, position: 'relative' }} />
      )}
    </>
  );
};

export default UnitIcon;
