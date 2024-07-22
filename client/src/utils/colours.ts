import baseColours from 'tailwindcss/colors';

const colours = {
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

  orderNew: baseColours.black,
  orderSuccess: baseColours.black,
  orderFailure: baseColours.red[500],
  orderRetreat: baseColours.orange[400],
  orderHighlight: baseColours.lime[500],

  sea: baseColours.sky[200],
  seaHover: baseColours.sky[300],
};

export default colours;
