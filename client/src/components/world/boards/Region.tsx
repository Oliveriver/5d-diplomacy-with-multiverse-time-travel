import { useContext, useEffect, useState } from 'react';
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
import BuildOptions from '../../user-interface/BuildOption';
import colours from '../../../utils/colours';
import RegionType from '../../../types/enums/regionType';
import useSelectLocation from '../../../hooks/useSelectLocation';

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
  isVisible?: boolean;
};

const Region = ({ id, timeline, year, phase, owner, unit, isVisible = true }: RegionProps) => {
  const { dispatch, currentOrder } = useContext(OrderEntryContext);

  const location = { timeline, year, phase, region: id };
  const Svg = useRegionSvg(id)!;
  const { x, y, type } = regions[id];

  const canSelect = useSelectLocation(location, owner, unit);

  const scaleFactor =
    phase === Phase.Winter
      ? (minorBoardWidth - boardBorderWidth * 2) / (majorBoardWidth - boardBorderWidth * 2)
      : 1;

  const [isHovering, setIsHovering] = useState(false);

  useEffect(() => {
    if (!isVisible) setIsHovering(false);
  }, [isVisible]);

  const isSelected =
    compareLocations(currentOrder?.location, location) ||
    (currentOrder?.$type === OrderType.Support &&
      compareLocations(currentOrder.supportLocation, location)) ||
    (currentOrder?.$type === OrderType.Convoy &&
      compareLocations(currentOrder.convoyLocation, location));

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
    <div title={id}>
      {isVisible && (
        <Svg
          className="absolute w-full h-full"
          cursor={canSelect ? 'pointer' : undefined}
          onClick={select}
          color={colour}
          onMouseEnter={() => setIsHovering(canSelect)}
          onMouseLeave={() => setIsHovering(false)}
          data-type="region"
          data-owner={owner}
          data-is-selected={isSelected}
          data-is-hovering={isHovering}
          data-region-type={type}
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
    </div>
  );
};

export default Region;
