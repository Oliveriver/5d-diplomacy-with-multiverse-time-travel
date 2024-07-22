import { useContext } from 'react';
import regions from '../../data/regions';
import Nation, { getNationColour } from '../../types/enums/nation';
import { getLatestPhase } from '../../types/enums/phase';
import colours from '../../utils/colours';
import { filterUnique } from '../../utils/listUtils';
import WorldContext from '../context/WorldContext';
import GameDetails from './GameDetails';

const PlayerList = () => {
  const { world } = useContext(WorldContext);
  if (!world) return null;

  const timelines = filterUnique(world.boards.map(({ timeline }) => timeline));
  const activeBoards = timelines.map((timeline) =>
    world.boards
      .filter((board) => board.timeline === timeline)
      .reduce((board1, board2) => {
        if (board1.year > board2.year) return board1;
        if (board2.year > board1.year) return board2;

        return getLatestPhase(board1.phase, board2.phase) === board1.phase ? board1 : board2;
      }),
  );

  // TODO sort out new win condition
  const totalCentres =
    activeBoards.length * Object.values(regions).filter((region) => region.isSupplyCentre).length;

  const winPercentages = Object.values(Nation)
    .map((nation) => {
      const centreCount = activeBoards.flatMap((board) =>
        Object.values(board.centres).filter((owner) => owner === nation),
      ).length;

      return {
        nation,
        percentage: 100 * (centreCount / totalCentres),
      };
    })
    .sort((player1, player2) => player2.percentage - player1.percentage);

  return (
    <div className="absolute left-10 top-10 flex flex-col gap-4">
      <GameDetails />
      <div
        className="flex flex-col gap-2 p-4 rounded w-max"
        style={{ backgroundColor: colours.uiOverlay }}
      >
        {winPercentages.map(({ nation, percentage }) => (
          <div
            key={nation}
            className="text-lg flex"
            style={{
              opacity: percentage > 0 ? 1 : 0.3,
              color: getNationColour(nation),
            }}
          >
            <p className="min-w-14 font-bold">{`${Math.round(percentage)}%`}</p>
            <p>{nation}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default PlayerList;
