import { useContext, useEffect, useMemo, useRef, useState } from 'react';
import Board from '../../../types/board';
import Region, { rasteriseRegion, RegionProps } from './Region';
import regions from '../../../data/regions';
import OrderEntryContext from '../../context/OrderEntryContext';
import UnitType from '../../../types/enums/unitType';
import InputMode from '../../../types/enums/inputMode';
import Order, { OrderType } from '../../../types/order';
import WorldContext from '../../context/WorldContext';
import World, { findUnit } from '../../../types/world';
import Phase from '../../../types/enums/phase';
import {
  boardBorderWidth,
  majorBoardWidth,
  minorBoardWidth,
  rasteriseDisplayFallback,
  rasteriseEnabled,
  rasteriseFactor,
  rasteriseScaleThreshold,
} from '../../../utils/constants';
import WorkQueueContext from '../../context/WorkQueueContext';
import ScaleContext from '../../context/ScaleContext';
import DarkModeContext from '../../context/DarkModeContext';

type MapProps = {
  board: Board;
  isShowingCoasts?: boolean;
};

const getRegions = (
  board: Board,
  isShowingCoasts: boolean,
  world: World | null,
  currentMode: InputMode,
  currentOrder: Order | null,
): RegionProps[] =>
  Object.keys(regions).map((region) => {
    const baseRegion = region.split('_')[0];
    const isCoast = region !== baseRegion;

    if (!isCoast) {
      return {
        id: region,
        timeline: board.timeline,
        year: board.year,
        phase: board.phase,
        owner: board.centres[region],
        unit: board.units[region],
      };
    }

    const hasArmy = board.units[baseRegion] !== undefined;
    const hasFleet = board.units[region] !== undefined;

    const showCoast = {
      [InputMode.None]: hasFleet || (board.phase === Phase.Winter && !hasArmy),
      [InputMode.Hold]: hasFleet,
      [InputMode.Move]: currentOrder?.unit?.type === UnitType.Fleet,
      [InputMode.Support]:
        currentOrder?.$type === OrderType.Support &&
        ((!currentOrder.supportLocation && hasFleet) ||
          (currentOrder.supportLocation !== null &&
            findUnit(world, currentOrder.supportLocation)?.type === UnitType.Fleet)),
      [InputMode.Convoy]: hasFleet,
      [InputMode.Build]: true,
      [InputMode.Disband]: hasFleet,
    }[currentMode];

    return {
      id: region,
      timeline: board.timeline,
      year: board.year,
      phase: board.phase,
      owner: board.centres[baseRegion],
      unit: board.units[region],
      isVisible: showCoast || isShowingCoasts,
    };
  });

const Map = ({ board, isShowingCoasts = false }: MapProps) => {
  const { world } = useContext(WorldContext);
  const { currentMode, currentOrder } = useContext(OrderEntryContext);
  const { scale } = useContext(ScaleContext);
  const isDarkMode = useContext(DarkModeContext);

  const containerRef = useRef<HTMLDivElement>(null);
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => setIsVisible(entry.isIntersecting));
    });
    const { current } = containerRef;
    if (current) observer.observe(current);
    return () => {
      if (current) observer.unobserve(current);
    };
  }, []);

  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [rasterisedInDarkMode, setRasterisedInDarkMode] = useState<boolean | null>(null);
  const rasteriseQueue = useContext(WorkQueueContext);

  useEffect(() => {
    if (!rasteriseEnabled) return;

    rasteriseQueue.push(
      (onComplete) => {
        if (
          canvasRef.current === null ||
          canvasRef.current.getAttribute('data-dark-mode') === isDarkMode.toString()
        ) {
          onComplete();
          return;
        }

        // Create a rasterised copy of the map onto a canvas element.
        // When zoomed out, we can stop showing the individual SVGs and instead show a single image.
        // This helps browser responsiveness as it reduces the number of DOM elements when zoomed out.
        let rasteriseSize =
          board.phase === Phase.Winter
            ? minorBoardWidth - boardBorderWidth * 2
            : majorBoardWidth - boardBorderWidth * 2;
        rasteriseSize *= rasteriseScaleThreshold * rasteriseFactor;

        const canvas = canvasRef.current;
        canvas.width = rasteriseSize;
        canvas.height = rasteriseSize;

        const context = canvas.getContext('2d')!;
        context.imageSmoothingEnabled = true;
        context.imageSmoothingQuality = 'high';

        context.fillStyle = window.getComputedStyle(canvas).getPropertyValue('--board-background');
        context.fillRect(0, 0, canvas.width, canvas.height);

        const regionProps = getRegions(board, isShowingCoasts, world, currentMode, currentOrder);
        let regionsLeft = regionProps.length;
        Object.values(regionProps).forEach((regionProp) => {
          rasteriseRegion(
            rasteriseQueue.sharedCache as Map<string, Promise<CanvasImageSource>>,
            regionProp,
            canvas,
            context,
            () => {
              regionsLeft--;
              if (regionsLeft === 0) {
                canvas.setAttribute('data-dark-mode', isDarkMode.toString());
                setRasterisedInDarkMode(isDarkMode);
                onComplete();
              }
            },
          );
        });
      },
      () => {
        // Rank items by distance to window centre, so those nearer the middle of screen get rendered first.
        const canvas = canvasRef.current;
        if (!canvas)
          return window.innerWidth * window.innerWidth + window.innerHeight * window.innerHeight;
        const rect = canvas.getBoundingClientRect();
        const midX = rect.x + rect.width / 2;
        const midY = rect.y + rect.height / 2;
        const xDist = window.innerWidth / 2 - midX;
        const yDist = window.innerHeight / 2 - midY;
        return xDist * xDist + yDist * yDist;
      },
    );
  }, [board, currentMode, currentOrder, isShowingCoasts, rasteriseQueue, world, isDarkMode]);

  const isRasterisedForCurrentMode = rasterisedInDarkMode === isDarkMode;
  const showRasterised = rasteriseEnabled && scale < rasteriseScaleThreshold;

  const mapRegions = useMemo(
    () => (
      <div
        id={`map-${board.timeline}-${board.year}-${board.phase}`}
        className="w-full h-full"
        ref={containerRef}
      >
        {rasteriseEnabled && (
          <canvas
            className="absolute w-full h-full"
            ref={canvasRef}
            style={{
              visibility:
                isRasterisedForCurrentMode && (showRasterised || !isVisible) ? 'visible' : 'hidden',
            }}
          />
        )}
        {(showRasterised && (isRasterisedForCurrentMode || !rasteriseDisplayFallback)) || !isVisible
          ? null // Remove SVGs from DOM, to improve responsiveness when zoomed out or map offscreen.
          : getRegions(board, isShowingCoasts, world, currentMode, currentOrder).map((props) => (
            <Region key={props.id} {...props} />
          ))}
      </div>
    ),
    [
      board,
      currentMode,
      currentOrder,
      isShowingCoasts,
      isVisible,
      isRasterisedForCurrentMode,
      showRasterised,
      world,
    ],
  );

  return mapRegions;
};

export default Map;
