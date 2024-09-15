import { boardSeparation, initialScale, majorBoardWidth } from './constants';

export const getDefaultOffsetX = () =>
  (window.innerWidth - initialScale * (majorBoardWidth + boardSeparation)) / 2;

export const getDefaultOffsetY = () =>
  (window.innerHeight - initialScale * (majorBoardWidth + boardSeparation)) / 2 - 40;
