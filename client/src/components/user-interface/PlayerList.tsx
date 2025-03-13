import { useContext, useRef, useState } from 'react';
import Nation from '../../types/enums/nation';
import colours from '../../utils/colours';
import WorldContext from '../context/WorldContext';
import GameDetails from './GameDetails';
import { filterUnique } from '../../utils/listUtils';
import PlayerListItem from './PlayerListItem';
import GameContext from '../context/GameContext';

const PlayerList = () => {
  const { playersSubmitted } = useContext(GameContext);
  const { world, boardState } = useContext(WorldContext);

  const scrollRef = useRef<HTMLDivElement>(null);
  const [expandedPlayers, setExpandedPlayers] = useState<Nation[]>([]);

  if (!world || !boardState) return null;
  const { winner } = world;

  const playerCentres = Object.values(Nation)
    .map((nation) => ({
      player: nation,
      centres: filterUnique(
        boardState.activeBoards.flatMap((board) =>
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
          const hasSubmitted = playersSubmitted.includes(player);

          return (
            <PlayerListItem
              key={player}
              player={player}
              centres={centres}
              hasSubmitted={hasSubmitted}
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
