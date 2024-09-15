import { useContext, useRef, useState } from 'react';
import Nation from '../../types/enums/nation';
import colours from '../../utils/colours';
import WorldContext from '../context/WorldContext';
import GameDetails from './GameDetails';
import { getActiveBoards } from '../../types/board';
import { filterUnique } from '../../utils/listUtils';
import PlayerListItem from './PlayerListItem';

const PlayerList = () => {
  const { world } = useContext(WorldContext);
  const scrollRef = useRef<HTMLDivElement>(null);
  const [expandedPlayers, setExpandedPlayers] = useState<Nation[]>([]);

  if (!world) return null;
  const { winner } = world;

  const activeBoards = getActiveBoards(world.boards);
  const playerCentres = Object.values(Nation)
    .map((nation) => ({
      player: nation,
      centres: filterUnique(
        activeBoards.flatMap((board) =>
          Object.keys(board.centres)
            .filter((region) => board.centres[region] === nation)
            .sort(),
        ),
      ),
    }))
    .sort((player1, player2) => player2.centres.length - player1.centres.length);

  const maxHeight = window.innerHeight - 296;

  return (
    <div className="absolute left-10 top-10 flex flex-col gap-4">
      <GameDetails />
      <div
        ref={scrollRef}
        className="flex flex-col gap-2 p-4 rounded w-max"
        style={{
          backgroundColor: colours.uiOverlay,
          maxHeight,
          overflowY: 'auto',
        }}
      >
        {playerCentres.map(({ player, centres }) => {
          const isExpanded = expandedPlayers.includes(player);
          return (
            <PlayerListItem
              key={player}
              player={player}
              centres={centres}
              winner={winner}
              isExpanded={isExpanded}
              toggleExpand={() =>
                setExpandedPlayers(
                  isExpanded
                    ? expandedPlayers.filter((nation) => nation !== player)
                    : [...expandedPlayers, player],
                )
              }
            />
          );
        })}
      </div>
    </div>
  );
};

export default PlayerList;
