import { useContext, useState } from 'react';
import Nation, { getNationColour } from '../../../types/enums/nation';
import OrderEntryContext from '../../context/OrderEntryContext';
import { OrderEntryActionType } from '../../../types/context/orderEntryAction';
import Unit from '../../../types/unit';
import useRegionSvg from '../../../hooks/useRegionSvg';
import regions from '../../../data/regions';
import Phase from '../../../types/enums/phase';
import { boardBorderWidth, majorBoardWidth, minorBoardWidth } from '../../../utils/constants';
import UnitIcon from './UnitIcon';
import { compareLocations, getLocationKey } from '../../../types/location';
import { OrderType } from '../../../types/order';
import BuildOptions from '../../interface/BuildOption';
import InputMode from '../../../types/enums/inputMode';
import colours from '../../../utils/colours';
import RegionType from '../../../types/enums/regionType';

const getRegionColour = (
  isHovering: boolean,
  isSelected: boolean,
  owner?: Nation,
  isSea?: boolean,
) => {
  if (isSelected) return colours.orderHighlight;
  if (isSea) return isHovering ? colours.seaHover : colours.sea;
  return getNationColour(owner, isHovering);
};

type RegionProps = {
  id: string;
  timeline: number;
  year: number;
  phase: Phase;
  owner?: Nation;
  unit?: Unit;
  isActive: boolean;
  isVisible?: boolean;
};

const Region = ({
  id,
  timeline,
  year,
  phase,
  owner,
  unit,
  isActive,
  isVisible = true,
}: RegionProps) => {
  const { dispatch, currentOrder, currentMode } = useContext(OrderEntryContext);

  const location = { timeline, year, phase, region: id };
  const Svg = useRegionSvg(id)!;
  const { x, y, type, homeNation } = regions[id];

  const canRetreat = unit?.mustRetreat;
  const canMove = isActive && phase !== Phase.Winter && unit !== undefined;
  const canBuild =
    isActive &&
    phase === Phase.Winter &&
    currentMode !== InputMode.Disband &&
    unit === undefined &&
    homeNation !== undefined &&
    owner === homeNation;
  const canDisband = isActive && phase === Phase.Winter && unit !== undefined;

  const canCreateOrder = canRetreat || canMove || canBuild || canDisband;

  const hasUnfinishedOrder =
    currentOrder?.location !== undefined && currentOrder.$type !== OrderType.Build;
  const canSelect = canCreateOrder || hasUnfinishedOrder;

  const scaleFactor =
    phase === Phase.Winter
      ? (minorBoardWidth - boardBorderWidth * 2) / (majorBoardWidth - boardBorderWidth * 2)
      : 1;

  const [isHovering, setIsHovering] = useState(false);

  let isSelected = compareLocations(currentOrder?.location, location);
  if (currentOrder?.$type === OrderType.Support) {
    isSelected ||= compareLocations(currentOrder.supportLocation, location);
  } else if (currentOrder?.$type === OrderType.Convoy) {
    isSelected ||= compareLocations(currentOrder.convoyLocation, location);
  }

  const colour = getRegionColour(isHovering, isSelected, owner, type === RegionType.Sea);

  const select = () => {
    if (!canSelect) return;

    dispatch({
      $type: OrderEntryActionType.Add,
      unit,
      location,
    });
  };

  return (
    <>
      {isVisible && (
        <Svg
          className="absolute w-full h-full"
          cursor={canSelect ? 'pointer' : undefined}
          onClick={select}
          color={colour}
          onMouseEnter={() => setIsHovering(canSelect)}
          onMouseLeave={() => setIsHovering(false)}
        />
      )}
      <div
        id={getLocationKey(location)}
        className="absolute"
        style={{
          left: x * scaleFactor,
          bottom: y * scaleFactor,
        }}
      >
        {unit && <UnitIcon unit={unit} scaleFactor={scaleFactor} />}
        <BuildOptions location={location} owner={owner} scaleFactor={scaleFactor} />
      </div>
    </>
  );
};

export default Region;
