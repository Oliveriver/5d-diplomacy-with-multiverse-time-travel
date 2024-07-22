import { useContext, useEffect } from 'react';
import Phase, { getLatestPhase } from '../types/enums/phase';
import { filterUnique } from '../utils/listUtils';
import InputMode from '../types/enums/inputMode';
import { OrderEntryActionType } from '../types/context/orderEntryAction';
import OrderEntryContext from '../components/context/OrderEntryContext';
import WorldContext from '../components/context/WorldContext';

const useSetAvailableInputModes = () => {
  const { world, isLoading, error } = useContext(WorldContext);
  const { dispatch } = useContext(OrderEntryContext);

  const boards = world && !isLoading && !error ? world.boards : [];
  const winner = world?.winner;

  const timelines = filterUnique(boards.map(({ timeline }) => timeline));
  const activeBoards = timelines.map((timeline) =>
    boards
      .filter((board) => board.timeline === timeline)
      .reduce((board1, board2) => {
        if (board1.year > board2.year) return board1;
        if (board2.year > board1.year) return board2;

        return getLatestPhase(board1.phase, board2.phase) === board1.phase ? board1 : board2;
      }),
  );

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
