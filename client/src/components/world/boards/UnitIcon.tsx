import { getNationColour } from '../../../types/enums/nation';
import Unit, { displayUnit } from '../../../types/unit';
import { unitWidth } from '../../../utils/constants';

type UnitIconProps = {
  unit: Unit;
  scaleFactor: number;
};

const UnitIcon = ({ unit, scaleFactor }: UnitIconProps) => (
  <div
    className="absolute flex justify-center items-center rounded-full pointer-events-none"
    style={{
      backgroundColor: getNationColour(unit.owner),
      boxShadow: unit.mustRetreat ? '0px 0px 20px 15px red' : '0px 0px 2px black',
      width: unitWidth * scaleFactor,
      height: unitWidth * scaleFactor,
      margin: -(unitWidth * scaleFactor) / 2,
      zIndex: unit.mustRetreat ? 30 : 10,
    }}
  >
    <p style={{ fontSize: 16 * scaleFactor }}>{displayUnit(unit)}</p>
  </div>
);

export default UnitIcon;
