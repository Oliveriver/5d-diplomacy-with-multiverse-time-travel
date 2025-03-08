import { useContext } from 'react';
import WorldContext from '../components/context/WorldContext';
import OrderEntryContext from '../components/context/OrderEntryContext';
import Location from '../types/location';
import Phase from '../types/enums/phase';
import World, { findUnit } from '../types/world';
import Order from '../types/order';
import InputMode from '../types/enums/inputMode';
import regions from '../data/regions';
import GameContext from '../components/context/GameContext';
import Nation from '../types/enums/nation';
import Unit from '../types/unit';

const canSelectMajorLocation = (
  player: Nation | null,
  currentOrder: Order | null,
  currentMode: InputMode,
  unit: Unit | undefined,
  isActiveBoard: boolean,
  isRetreatTurn: boolean,
) => {
  if (currentMode === InputMode.Build) return false;

  if (isRetreatTurn) {
    const canStartRetreat =
      (currentOrder === null || currentOrder.unit === null) &&
      (currentMode === InputMode.None ||
        currentMode === InputMode.Move ||
        currentMode === InputMode.Disband) &&
      unit !== undefined &&
      unit.mustRetreat;
    const canFinishRetreat =
      currentOrder !== null && currentOrder.unit !== null && currentMode === InputMode.Move;

    return canStartRetreat || canFinishRetreat;
  }

  if (currentMode === InputMode.Disband) return false;

  const canStartOrder =
    isActiveBoard &&
    unit !== undefined &&
    (!player || player === unit?.owner) &&
    (currentOrder === null || currentOrder.unit === undefined) &&
    !unit.mustRetreat;
  const canFinishOrder = currentOrder !== null && currentOrder.unit !== null;

  return canStartOrder || canFinishOrder;
};

const canSelectMinorLocation = (
  player: Nation | null,
  world: World,
  currentOrder: Order | null,
  currentMode: InputMode,
  location: Location,
  owner: Nation | undefined,
  unit: Unit | undefined,
  isActiveBoard: boolean,
  isRetreatTurn: boolean,
) => {
  if (!isActiveBoard || isRetreatTurn) return false;

  const coasts = Object.keys(regions).filter(
    (region) => region !== location.region && region.includes(location.region),
  );
  const unitIncludingCoasts =
    unit ?? coasts.find((coast) => findUnit(world, { ...location, region: coast }));

  if (!unitIncludingCoasts && (currentMode === InputMode.Build || currentMode === InputMode.None)) {
    const baseRegion = location.region.split('_')[0];
    const { isSupplyCentre, homeNation } = regions[baseRegion];
    return (
      isSupplyCentre === true &&
      homeNation !== undefined &&
      (!player || player === homeNation) &&
      owner === homeNation &&
      currentOrder === null
    );
  }

  if (
    currentMode === InputMode.Disband ||
    (unitIncludingCoasts && currentMode === InputMode.None)
  ) {
    return unit !== undefined && (!player || player === unit.owner);
  }

  return false;
};

const useSelectLocation = (
  location: Location,
  owner: Nation | undefined,
  unit: Unit | undefined,
): boolean => {
  const { game } = useContext(GameContext);
  const { world, isLoading, boardState } = useContext(WorldContext);
  const { currentOrder, currentMode } = useContext(OrderEntryContext);

  if (!game || !world || isLoading || !boardState) return false;

  const { player } = game;
  const { timeline, year, phase } = location;
  const isActiveBoard =
    !world.winner &&
    boardState.activeBoards.some(
      (board) => board.timeline === timeline && board.year === year && board.phase === phase,
    );

  return location.phase === Phase.Winter
    ? canSelectMinorLocation(
        player,
        world,
        currentOrder,
        currentMode,
        location,
        owner,
        unit,
        isActiveBoard,
        boardState.isRetreatTurn,
      )
    : canSelectMajorLocation(
        player,
        currentOrder,
        currentMode,
        unit,
        isActiveBoard,
        boardState.isRetreatTurn,
      );
};

export default useSelectLocation;
