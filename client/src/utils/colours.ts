import baseColours from 'tailwindcss/colors';
import { Colours } from './types';

export const lightColours: Colours = {
  uiForeground: baseColours.black, // Text colour
  uiBackground: baseColours.gray[300], // Background of UI elements
  uiPageBackground: baseColours.white, // Overriden on the homepage with an image
  uiOverlay: baseColours.white,
  uiHighlight: baseColours.lime[500],
  uiBorder: baseColours.gray[200], // Border of UI elements (buttons, inputs, etc.)
  uiError: baseColours.red[600],

  iconForeground: baseColours.white,
  iconDelete: baseColours.red[500],
  supplyCenter: baseColours.white,
  supplyCenterBorder: baseColours.white,

  boardBorder: baseColours.gray[200],
  boardBackground: baseColours.white, // Also controls colour of impassable terrain
  boardArrowHead: baseColours.green[300],
  boardArrowBody: baseColours.green[200],
  boardCountryBorder: baseColours.white,
  yearDivider: baseColours.indigo[50],
  yearDividerText: baseColours.indigo[200],

  orderNew: baseColours.black,
  orderSuccess: baseColours.black,
  orderFailure: baseColours.red[600],
  orderRetreat: baseColours.orange[600],
  orderHighlight: baseColours.lime[500],

  sea: baseColours.sky[200],
  seaHover: baseColours.sky[300],

  unclaimedStandard: baseColours.orange[200],
  unclaimedEmphasised: baseColours.orange[300],

  englandStandard: baseColours.fuchsia[400],
  englandEmphasised: baseColours.fuchsia[500],
  germanyStandard: baseColours.zinc[400],
  germanyEmphasised: baseColours.zinc[500],
  russiaStandard: baseColours.purple[400],
  russiaEmphasised: baseColours.purple[500],
  turkeyStandard: baseColours.amber[400],
  turkeyEmphasised: baseColours.amber[500],
  austriaStandard: baseColours.red[400],
  austriaEmphasised: baseColours.red[500],
  italyStandard: baseColours.emerald[400],
  italyEmphasised: baseColours.emerald[500],
  franceStandard: baseColours.blue[400],
  franceEmphasised: baseColours.blue[500],
};

export const darkColours: Colours = {
  ...lightColours,

  uiForeground: baseColours.white,
  uiBackground: baseColours.gray[800],
  uiPageBackground: baseColours.gray[950],
  uiOverlay: baseColours.gray[800],
  uiBorder: baseColours.gray[700],

  boardBorder: baseColours.gray[700],
  boardBackground: baseColours.zinc[950],
  boardCountryBorder: baseColours.zinc[950],
  yearDivider: baseColours.indigo[900],
  yearDividerText: baseColours.indigo[800],

  orderNew: baseColours.white,
  orderSuccess: baseColours.white,

  sea: baseColours.sky[900],
  seaHover: baseColours.sky[800],

  unclaimedStandard: '#5D4532',
  unclaimedEmphasised: '#6A4F39',

  englandStandard: baseColours.fuchsia[700],
  englandEmphasised: baseColours.fuchsia[600],
  germanyStandard: baseColours.zinc[500],
  germanyEmphasised: baseColours.zinc[400],
  russiaStandard: baseColours.purple[600],
  russiaEmphasised: baseColours.purple[500],
  turkeyStandard: baseColours.amber[600],
  turkeyEmphasised: baseColours.amber[500],
  austriaStandard: baseColours.red[800],
  austriaEmphasised: baseColours.red[700],
  italyStandard: baseColours.emerald[600],
  italyEmphasised: baseColours.emerald[500],
  franceStandard: baseColours.blue[600],
  franceEmphasised: baseColours.blue[500],
};

export const coloursVariables: Colours = {
  uiForeground: '--ui-foreground',
  uiBackground: '--ui-background',
  uiPageBackground: '--ui-page-background',
  uiOverlay: '--ui-overlay',
  uiHighlight: '--ui-highlight',
  uiBorder: '--ui-border',
  uiError: '--ui-error',
  iconForeground: '--icon-foreground',
  iconDelete: '--icon-delete',
  supplyCenter: '--supply-center',
  supplyCenterBorder: '--supply-center-border',
  boardBorder: '--board-border',
  boardBackground: '--board-background',
  boardArrowHead: '--board-arrow-head',
  boardArrowBody: '--board-arrow-body',
  boardCountryBorder: '--board-country-border',
  yearDivider: '--year-divider',
  yearDividerText: '--year-divider-text',
  orderNew: '--order-new',
  orderSuccess: '--order-success',
  orderFailure: '--order-failure',
  orderRetreat: '--order-retreat',
  orderHighlight: '--order-highlight',
  sea: '--sea',
  seaHover: '--sea-hover',
  unclaimedStandard: '--unclaimed-standard',
  unclaimedEmphasised: '--unclaimed-emphasised',
  englandStandard: '--england-standard',
  englandEmphasised: '--england-emphasised',
  germanyStandard: '--germany-standard',
  germanyEmphasised: '--germany-emphasised',
  russiaStandard: '--russia-standard',
  russiaEmphasised: '--russia-emphasised',
  turkeyStandard: '--turkey-standard',
  turkeyEmphasised: '--turkey-emphasised',
  austriaStandard: '--austria-standard',
  austriaEmphasised: '--austria-emphasised',
  italyStandard: '--italy-standard',
  italyEmphasised: '--italy-emphasised',
  franceStandard: '--france-standard',
  franceEmphasised: '--france-emphasised',
};

const coloursCssVariables = Object.entries(coloursVariables).reduce(
  (acc, [key, value]) => ({ ...acc, [key]: `var(${value})` }),
  {},
) as Colours;

export default coloursCssVariables;
