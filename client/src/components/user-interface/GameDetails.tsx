import { useContext } from 'react';
import colours from '../../utils/colours';
import { getNationColour } from '../../types/enums/nation';
import CopyIcon from '../../assets/icons/CopyIcon.svg?react';
import DownloadIcon from '../../assets/icons/DownloadIcon.svg?react';
import GameContext from '../context/GameContext';
import WorldContext from '../context/WorldContext';
import SaveFile from '../../types/saveFile';
import { OrderStatus } from '../../types/order';

const GameDetails = () => {
  const { game } = useContext(GameContext);
  const { world } = useContext(WorldContext);

  if (!game) return null;
  const { id, player } = game;

  const onGameIdCopied = () => navigator.clipboard.writeText(id.toString());

  const adjacencySettingText = `Adjacencies: ${game.hasStrictAdjacencies ? 'Strict' : 'Loose'}`;

  let gameJsonFile: string | null = null;
  if (world) {
    const saveFile: SaveFile = {
      hasStrictAdjacencies: game.hasStrictAdjacencies,
      iteration: world.iteration,
      orders: world.orders.filter(
        (o) => o.status !== OrderStatus.New && o.status !== OrderStatus.RetreatNew,
      ),
    };

    gameJsonFile = URL.createObjectURL(new Blob([JSON.stringify(saveFile, null, 2)]));
  }

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
        {gameJsonFile && (
          <a
            aria-label="Download game as JSON"
            className="opacity-30 hover:opacity-70 ml-6"
            href={gameJsonFile}
            download="game.json"
            target="_blank"
            rel="noreferrer"
            title="Download game as JSON"
          >
            <DownloadIcon />
          </a>
        )}
      </div>
      <p
        className="font-bold text-lg"
        style={{ color: player ? getNationColour(player) : colours.uiForeground }}
      >
        {player ?? 'Sandbox'}
      </p>
      <p className="text-xs opacity-50">{adjacencySettingText}</p>
    </div>
  );
};

export default GameDetails;
