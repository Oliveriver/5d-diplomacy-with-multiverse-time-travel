import regions from '../data/regions';
import {
  boardBorderWidth,
  boardSeparation,
  majorBoardWidth,
  minorBoardWidth,
  startYear,
} from '../utils/constants';
import Phase from './enums/phase';

type Location = {
  timeline: number;
  year: number;
  phase: Phase;
  region: string;
};

export const isCoast = (region: string) => {
  const baseRegion = region.split('_')[0];
  return baseRegion !== region;
};

export const displayLocation = (location: Location | null) =>
  location && `(${location.timeline}) ${location.phase} ${location.year} ${location.region}`;

export const getLocationKey = (location: Location) =>
  `${location.timeline} ${location.year} ${location.phase} ${location.region}`;

export const compareLocations = (location1?: Location | null, location2?: Location | null) =>
  location1?.timeline === location2?.timeline &&
  location1?.year === location2?.year &&
  location1?.phase === location2?.phase &&
  location1?.region === location2?.region;

const getBaseCoordinates = (board: Omit<Location, 'region'>) => {
  const margin = boardSeparation / 2;

  const pastYearCount = Math.max(board.year - startYear, 0);
  const pastYearWidth = 3 * boardSeparation + 2 * majorBoardWidth + minorBoardWidth;

  const currentYearWidth = {
    [Phase.Spring]: boardSeparation,
    [Phase.Fall]: 2 * boardSeparation + majorBoardWidth,
    [Phase.Winter]: 3 * boardSeparation + 2 * majorBoardWidth,
  }[board.phase];

  const previousTimelineCount = board.timeline - 1;
  const previousTimelineHeight = boardSeparation + majorBoardWidth;

  const currentTimelineHeight = {
    [Phase.Spring]: boardSeparation,
    [Phase.Fall]: boardSeparation,
    [Phase.Winter]: boardSeparation + (majorBoardWidth - minorBoardWidth) / 2,
  }[board.phase];

  const x = pastYearCount * pastYearWidth + currentYearWidth - margin;
  const y = previousTimelineCount * previousTimelineHeight + currentTimelineHeight - margin;
  return { x, y };
};

export const getCoordinates = (location: Location) => {
  const { timeline, year, phase, region } = location;

  const boardBase = getBaseCoordinates({ timeline, year, phase });
  const boardWidth = phase === Phase.Winter ? minorBoardWidth : majorBoardWidth;

  if (region === 'centre') {
    return {
      x: boardBase.x + boardWidth / 2,
      y: boardBase.y + boardWidth / 2,
    };
  }

  const scale =
    phase === Phase.Winter
      ? (minorBoardWidth - 2 * boardBorderWidth) / (majorBoardWidth - 2 * boardBorderWidth)
      : 1;

  const offsetX = boardBorderWidth + regions[region].x * scale;
  const offsetY = boardWidth - boardBorderWidth - regions[region].y * scale;

  return {
    x: boardBase.x + offsetX,
    y: boardBase.y + offsetY,
  };
};

export default Location;
