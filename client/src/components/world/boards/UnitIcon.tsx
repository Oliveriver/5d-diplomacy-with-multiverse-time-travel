import { getNationColour } from '../../../types/enums/nation';
import Unit from '../../../types/unit';
import UnitType from '../../../types/enums/unitType';
import { unitWidth } from '../../../utils/constants';

import ArmyIcon from '../../../assets/icons/Army.svg?react';
import FleetIcon from '../../../assets/icons/Fleet.svg?react';

type UnitIconProps = {
  unit: Unit;
  scaleFactor?: number;
  variant?: 'world' | 'overlay';
};

const UnitIcon = ({ unit, scaleFactor = 1, variant = 'world' }: UnitIconProps) => {
  const isWorldVariant = variant === 'world';
  const shadow = unit.mustRetreat ? '0px 0px 2px red' : '0px 0px 2px black';
  const Svg = (unit.type === UnitType.Army ? ArmyIcon : FleetIcon);

  return (
    <Svg
      className="flex justify-center"
      style={{
        color: getNationColour(unit.owner),
        filter: isWorldVariant ? `drop-shadow(${shadow})` : '',
        width: unitWidth * scaleFactor,
        height: unitWidth * scaleFactor,
        margin: isWorldVariant ? -(unitWidth * scaleFactor) / 2 : 0,
        zIndex: unit.mustRetreat ? 30 : 10,
        position: isWorldVariant ? 'absolute' : 'relative',
        pointerEvents: isWorldVariant ? 'none' : 'auto',
      }}
    >
    </Svg>
  );
}


export default UnitIcon;
