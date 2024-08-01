import { useContext, useEffect } from 'react';
import Phase from '../types/enums/phase';
import InputMode from '../types/enums/inputMode';
import { OrderEntryActionType } from '../types/context/orderEntryAction';
import OrderEntryContext from '../components/context/OrderEntryContext';
import WorldContext from '../components/context/WorldContext';
import { getActiveBoards } from '../types/board';

const useSetAvailableInputModes = () => {
  const { world, isLoading, error } = useContext(WorldContext);
  const { dispatch } = useContext(OrderEntryContext);

  const boards = world && !isLoading && !error ? world.boards : [];
  const winner = world?.winner;

  const activeBoards = getActiveBoards(boards);

  const hasMajorBoard = !winner && activeBoards.some((board) => board.phase !== Phase.Winter);
  const hasMinorBoard = !winner && activeBoards.some((board) => board.phase === Phase.Winter);

  useEffect(() => {
    // TODO allow disband during retreats
    const majorModes = [InputMode.Hold, InputMode.Move, InputMode.Support, InputMode.Convoy];
    const minorModes = [InputMode.Build, InputMode.Disband];

    dispatch({
      $type: OrderEntryActionType.SetAvailableModes,
      modes: [
        InputMode.None,
        ...(hasMajorBoard ? majorModes : []),
        ...(hasMinorBoard ? minorModes : []),
      ],
    });
  }, [dispatch, hasMajorBoard, hasMinorBoard]);
};

export default useSetAvailableInputModes;
