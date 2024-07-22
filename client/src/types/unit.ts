import Nation from './enums/nation';
import UnitType from './enums/unitType';

type Unit = {
  owner: Nation;
  type: UnitType;
  mustRetreat: boolean;
};

export const displayUnit = (unit: Unit | null) => (unit?.type === UnitType.Fleet ? 'F' : 'A');

export default Unit;
