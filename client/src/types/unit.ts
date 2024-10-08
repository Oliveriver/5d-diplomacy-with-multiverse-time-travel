import Nation from './enums/nation';
import UnitType from './enums/unitType';

type Unit = {
  owner: Nation;
  type: UnitType;
  mustRetreat: boolean;
};

export default Unit;
