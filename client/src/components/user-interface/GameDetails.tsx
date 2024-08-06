import { useContext } from 'react';
import colours from '../../utils/colours';
import { getNationColour } from '../../types/enums/nation';
import CopyIcon from '../../assets/icons/CopyIcon.svg?react';
import GameContext from '../context/GameContext';

const GameDetails = () => {
  const { game } = useContext(GameContext);

  if (!game) return null;
  const { id, player } = game;

  const onGameIdCopied = () => navigator.clipboard.writeText(id.toString());

  return (
    <div
      className="flex flex-col gap-2 rounded p-4 w-max"
      style={{ backgroundColor: colours.uiOverlay }}
    >
      <div className="flex flex-row items-center text-sm">
        <p title={JSON.stringify(game)}>Game ID:</p>
        <p className="ml-3 mr-1">{id}</p>
        <button
          type="button"
          aria-label="Copy to clipboard"
          className="opacity-30 hover:opacity-70"
          onClick={onGameIdCopied}
          title="Copy to clipboard"
        >
          <CopyIcon />
        </button>
      </div>
      <p
        className="font-bold text-lg"
        style={{ color: player ? getNationColour(player) : colours.uiForeground }}
      >
        {player ?? 'Sandbox'}
      </p>
    </div>
  );
};

export default GameDetails;
