import { useContext, useEffect } from 'react';
import Phase from '../types/enums/phase';
import InputMode from '../types/enums/inputMode';
import { OrderEntryActionType } from '../types/context/orderEntryAction';
import OrderEntryContext from '../components/context/OrderEntryContext';
import WorldContext from '../components/context/WorldContext';

const useSetAvailableInputModes = () => {
  const { world, isLoading, error, boardState } = useContext(WorldContext);
  const { dispatch } = useContext(OrderEntryContext);

  const activeBoardsOrEmpty =
    world && !isLoading && !error && boardState?.activeBoards ? boardState.activeBoards : [];
  const winner = world?.winner;

  const hasRetreats = activeBoardsOrEmpty.some((board) =>
    Object.values(board.units).some((unit) => unit.mustRetreat),
  );
  const hasMajorBoard =
    !winner && activeBoardsOrEmpty.some((board) => board.phase !== Phase.Winter);
  const hasMinorBoard =
    !winner && activeBoardsOrEmpty.some((board) => board.phase === Phase.Winter);

  useEffect(() => {
    const retreatModes = [InputMode.Move, InputMode.Disband];
    const majorModes = [InputMode.Hold, InputMode.Move, InputMode.Support, InputMode.Convoy];
    const minorModes = [InputMode.Build, InputMode.Disband];

    dispatch({
      $type: OrderEntryActionType.SetAvailableModes,
      modes: [
        InputMode.None,
        ...(hasRetreats ? retreatModes : []),
        ...(hasMajorBoard && !hasRetreats ? majorModes : []),
        ...(hasMinorBoard && !hasRetreats ? minorModes : []),
      ],
    });
  }, [dispatch, hasRetreats, hasMajorBoard, hasMinorBoard]);
};

export default useSetAvailableInputModes;
