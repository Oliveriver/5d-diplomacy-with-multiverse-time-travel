// Gameplay

export const startYear = 1901;
export const victoryRequiredCentreCount = 18;

// UI

export const initialScale = 0.8;
export const orderFocusScale = 1.5;
export const majorBoardWidth = 1000;
export const minorBoardWidth = 650;
export const boardSeparation = 400;
export const boardBorderWidth = 32;
export const unitWidth = 28;
export const boardArrowWidth = 200;
export const orderArrowStartSeparation = 20;
export const orderArrowEndSeparation = 10;
export const pastTurnOpacity = 0.6;

// Enable showing maps as rasterised images instead of SVGs. Improves UI responsiveness when zoomed out, but disables interactivity.
export const rasteriseEnabled = true;
// At what zoom level under which we display the images instead of the SVGs.
export const rasteriseScaleThreshold = 0.2;
// Size factor of the generated image at the threshold. 1 generates full resolution, 0.5 would be half resolution. Larger sizes improve quality, smaller reduces memory usage.
export const rasteriseFactor = 1;
// true = show SVGs as a fallback whilst the rasters generate; false = show empty map whilst rasters generate.
export const rasteriseDisplayFallback = true;

// API

export const iterationRefetchInterval = 2000;
export const playersSubmittedRefetchInterval = 10000;
