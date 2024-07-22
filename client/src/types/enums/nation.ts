import colours from 'tailwindcss/colors';

enum Nation {
  England = 'England',
  Germany = 'Germany',
  Russia = 'Russia',
  Turkey = 'Turkey',
  Austria = 'Austria',
  Italy = 'Italy',
  France = 'France',
}

export const getNationColour = (nation?: Nation, isEmphasised: boolean = true) => {
  if (!nation) return colours.orange[isEmphasised ? 300 : 200];
  return {
    [Nation.England]: colours.fuchsia[isEmphasised ? 500 : 400],
    [Nation.Germany]: colours.zinc[isEmphasised ? 500 : 400],
    [Nation.Russia]: colours.purple[isEmphasised ? 500 : 400],
    [Nation.Turkey]: colours.amber[isEmphasised ? 500 : 400],
    [Nation.Austria]: colours.red[isEmphasised ? 500 : 400],
    [Nation.Italy]: colours.emerald[isEmphasised ? 500 : 400],
    [Nation.France]: colours.blue[isEmphasised ? 500 : 400],
  }[nation];
};

export default Nation;
