import { getNationColour } from '../../../types/enums/nation';
import Unit, { displayUnit } from '../../../types/unit';
import { unitWidth } from '../../../utils/constants';

type UnitIconProps = {
  unit: Unit;
  scaleFactor?: number;
  variant?: 'world' | 'overlay';
};

const UnitIcon = ({ unit, scaleFactor = 1, variant = 'world' }: UnitIconProps) => {
  const isWorldVariant = variant === 'world';
  const shadow = unit.mustRetreat ? '0px 0px 20px 15px red' : '0px 0px 2px black';

  return (
    <div
      className="flex justify-center items-center rounded-full"
      style={{
        backgroundColor: getNationColour(unit.owner),
        boxShadow: isWorldVariant ? shadow : '',
        width: unitWidth * scaleFactor,
        height: unitWidth * scaleFactor,
        margin: isWorldVariant ? -(unitWidth * scaleFactor) / 2 : 0,
        zIndex: unit.mustRetreat ? 30 : 10,
        position: isWorldVariant ? 'absolute' : 'relative',
        pointerEvents: isWorldVariant ? 'none' : 'auto',
      }}
    >
      <p style={{ fontSize: 16 * scaleFactor }}>{displayUnit(unit)}</p>
    </div>
  );
};

export default UnitIcon;
