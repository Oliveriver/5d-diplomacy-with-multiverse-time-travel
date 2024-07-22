import Nation from './enums/nation';
import RegionType from './enums/regionType';

type Region = {
  x: number;
  y: number;
  type: RegionType;
  isSupplyCentre?: boolean;
  homeNation?: Nation;
};

export default Region;
