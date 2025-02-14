import { getNationColour } from '../../../types/enums/nation';
import Unit from '../../../types/unit';
import UnitType from '../../../types/enums/unitType';
import { unitWidth } from '../../../utils/constants';
import ArmyIcon from '../../../assets/icons/ArmyIcon.svg?react';
import FleetIcon from '../../../assets/icons/FleetIcon.svg?react';

type UnitIconProps = {
  unit: Unit;
  scaleFactor?: number;
  variant?: 'world' | 'overlay';
};

const UnitIcon = ({ unit, scaleFactor = 1, variant = 'world' }: UnitIconProps) => {
  const isWorldVariant = variant === 'world';
  const Svg = unit.type === UnitType.Army ? ArmyIcon : FleetIcon;

  const width = unitWidth * scaleFactor;

  return (
    <>
      <Svg
        className="flex justify-center overflow-visible"
        style={{
          color: getNationColour(unit.owner),
          width,
          height: width,
          margin: isWorldVariant ? -width / 2 : 0,
          zIndex: unit.mustRetreat ? 30 : 10,
          position: isWorldVariant ? 'absolute' : 'relative',
          pointerEvents: isWorldVariant ? 'none' : 'auto',
        }}
      />
      {isWorldVariant && unit.mustRetreat && (
        <div style={{ boxShadow: '0px 0px 35px 35px red', zIndex: 5, position: 'relative' }} />
      )}
    </>
  );
};

export default UnitIcon;
