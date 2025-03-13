import { useContext, useEffect, useState } from 'react';
import Nation, { getNationColour } from '../../../types/enums/nation';
import OrderEntryContext from '../../context/OrderEntryContext';
import { OrderEntryActionType } from '../../../types/context/orderEntryAction';
import Unit from '../../../types/unit';
import { useRegionSvg, getRegionSvgRaw } from '../../../hooks/useRegionSvg';
import regions from '../../../data/regions';
import Phase from '../../../types/enums/phase';
import { boardBorderWidth, majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import UnitIcon, { rasteriseUnitIcon } from './UnitIcon';
import { compareLocations, getLocationKey } from '../../../types/location';
import { OrderType } from '../../../types/order';
import BuildOptions from '../../user-interface/BuildOption';
import colours from '../../../utils/colours';
import RegionType from '../../../types/enums/regionType';
import useSelectLocation from '../../../hooks/useSelectLocation';

const getRegionColour = (
  isHovering: boolean,
  isSelected: boolean,
  owner?: Nation,
  isSea?: boolean,
) => {
  if (isSelected) return colours.orderHighlight;
  if (isSea) return isHovering ? colours.seaHover : colours.sea;
  return getNationColour(owner, isHovering);
};

export type RegionProps = {
  id: string;
  timeline: number;
  year: number;
  phase: Phase;
  owner?: Nation;
  unit?: Unit;
  isVisible?: boolean;
};

export const rasteriseRegion = (
  imageCache: Map<string, Promise<CanvasImageSource>>,
  { id, owner, unit, phase }: RegionProps,
  canvas: HTMLCanvasElement,
  context: CanvasRenderingContext2D,
  onComplete: () => void,
) => {
  const files = getRegionSvgRaw(id)!;

  let filesLeft = files.length;
  Object.values(files).forEach((file, index) => {
    const drawImage = (image: CanvasImageSource) => {
      context.drawImage(image, 0, 0, canvas.width, canvas.height);

      // Draw unit icons once region(s) are drawn.
      filesLeft--;
      if (filesLeft === 0) {
        if (unit) {
          const { x, y } = regions[id];
          const scaleFactor =
            phase === Phase.Winter
              ? (minorBoardWidth - boardBorderWidth * 2) / (majorBoardWidth - boardBorderWidth * 2)
              : 1;
          rasteriseUnitIcon(
            imageCache,
            { unit, scaleFactor },
            x * scaleFactor,
            y * scaleFactor,
            canvas,
            context,
            onComplete,
          );
        } else {
          onComplete();
        }
      }
    };

    // When drawing to canvas, CSS won't be applied, we'll need to apply the same effects manually.
    const computedStyle = window.getComputedStyle(canvas);

    let regionColour = getRegionColour(false, false, owner, regions[id].type === RegionType.Sea);
    regionColour = regionColour.replace('var(', '').replace(')', '');
    regionColour = computedStyle.getPropertyValue(regionColour);

    const cacheKey = `region|${id}|${index}|${regionColour}`;
    const cachedImage = imageCache.get(cacheKey);
    if (cachedImage !== undefined) {
      cachedImage.then(drawImage);
    } else {
      let svg = file;

      // Apply the SVG colour attribute.
      svg = svg.replace('"currentColor"', `"${regionColour}"`);

      // Apply CSS rules for regions from index.css.
      const boardCountryBorder = computedStyle.getPropertyValue('--board-country-border');
      const sea = computedStyle.getPropertyValue('--sea');
      const supplyCenter = computedStyle.getPropertyValue('--supply-center');
      const supplyCenterBorder = computedStyle.getPropertyValue('--supply-center-border');
      const svgDoc = new DOMParser().parseFromString(svg, 'image/svg+xml');
      svgDoc.querySelectorAll('path.sea-region').forEach((e) => e.setAttribute('fill', sea));
      svgDoc.querySelectorAll('path').forEach((e) => e.setAttribute('stroke', boardCountryBorder));
      svgDoc
        .querySelectorAll('g.supply-center-dot')
        .forEach((e) => e.setAttribute('fill', supplyCenter));
      svgDoc
        .querySelectorAll('g.supply-center-dot path')
        .forEach((e) => e.setAttribute('stroke', supplyCenterBorder));
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
  });
};

const Region = ({ id, timeline, year, phase, owner, unit, isVisible = true }: RegionProps) => {
  const { dispatch, currentOrder } = useContext(OrderEntryContext);

  const location = { timeline, year, phase, region: id };
  const Svg = useRegionSvg(id)!;
  const { x, y, type } = regions[id];

  const canSelect = useSelectLocation(location, owner, unit);

  const scaleFactor =
    phase === Phase.Winter
      ? (minorBoardWidth - boardBorderWidth * 2) / (majorBoardWidth - boardBorderWidth * 2)
      : 1;

  const [isHovering, setIsHovering] = useState(false);

  useEffect(() => {
    if (!isVisible) setIsHovering(false);
  }, [isVisible]);

  const isSelected =
    compareLocations(currentOrder?.location, location) ||
    (currentOrder?.$type === OrderType.Support &&
      compareLocations(currentOrder.supportLocation, location)) ||
    (currentOrder?.$type === OrderType.Convoy &&
      compareLocations(currentOrder.convoyLocation, location));

  const colour = getRegionColour(isHovering, isSelected, owner, type === RegionType.Sea);

  const select = () => {
    if (!canSelect) return;

    dispatch({
      $type: OrderEntryActionType.Add,
      unit,
      location,
    });
  };

  return (
    <div title={id}>
      {isVisible && (
        <Svg
          className="absolute w-full h-full"
          cursor={canSelect ? 'pointer' : undefined}
          onClick={select}
          color={colour}
          onMouseEnter={() => setIsHovering(canSelect)}
          onMouseLeave={() => setIsHovering(false)}
          data-type="region"
          data-owner={owner}
          data-is-selected={isSelected}
          data-is-hovering={isHovering}
          data-region-type={type}
        />
      )}
      <div
        id={getLocationKey(location)}
        className="absolute"
        style={{
          left: x * scaleFactor,
          bottom: y * scaleFactor,
        }}
      >
        {unit && <UnitIcon unit={unit} scaleFactor={scaleFactor} />}
        <BuildOptions location={location} owner={owner} scaleFactor={scaleFactor} />
      </div>
    </div>
  );
};

export default Region;
