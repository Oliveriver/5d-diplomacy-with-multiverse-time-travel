import themeColours from '../../utils/colours';

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
  if (!nation) return isEmphasised ? themeColours.unclaimedEmphasised : themeColours.unclaimedStandard;
  return {
    [Nation.England]: isEmphasised ? themeColours.englandEmphasised : themeColours.englandStandard,
    [Nation.Germany]: isEmphasised ? themeColours.germanyEmphasised : themeColours.germanyStandard,
    [Nation.Russia]: isEmphasised ? themeColours.russiaEmphasised : themeColours.russiaStandard,
    [Nation.Turkey]: isEmphasised ? themeColours.turkeyEmphasised : themeColours.turkeyStandard,
    [Nation.Austria]: isEmphasised ? themeColours.austriaEmphasised : themeColours.austriaStandard,
    [Nation.Italy]: isEmphasised ? themeColours.italyEmphasised : themeColours.italyStandard,
    [Nation.France]: isEmphasised ? themeColours.franceEmphasised : themeColours.franceStandard,
  }[nation];
};

export default Nation;
