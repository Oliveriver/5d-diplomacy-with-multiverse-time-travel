import baseColours from 'tailwindcss/colors';

const lightColours = {
  uiForeground: baseColours.black,
  uiBackground: baseColours.gray[300],
  uiOverlay: baseColours.white,
  uiBorder: baseColours.gray[200],
  uiHighlight: baseColours.lime[500],
  uiError: baseColours.red[600],

  iconForeground: baseColours.white,
  iconDelete: baseColours.red[500],

  boardBorder: baseColours.gray[200],
  boardBackground: baseColours.white,
  boardArrowHead: baseColours.green[300],
  boardArrowBody: baseColours.green[200],
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

const darkColours = {
  ...lightColours,

  uiForeground: baseColours.white,
  uiBackground: baseColours.gray[800],
  uiOverlay: baseColours.gray[700],
  uiBorder: baseColours.gray[700],

  boardBackground: baseColours.slate[700],

  orderNew: baseColours.white,
  orderSuccess: baseColours.white,

  sea: baseColours.sky[800],
  seaHover: baseColours.sky[900],

  germanyStandard: baseColours.zinc[300],
  germanyEmphasised: baseColours.zinc[400],
};

export const isDarkMode =
  window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;

const themeColours = isDarkMode ? darkColours : lightColours;

export default themeColours;

