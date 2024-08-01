import Nation, { getNationColour } from '../../types/enums/nation';
import { victoryRequiredCentreCount } from '../../utils/constants';
import ExpandButton from './common/ExpandButton';

type PlayerListItemProps = {
  player: Nation;
  centres: string[];
  winner: Nation | null;
  isExpanded: boolean;
  toggleExpand: () => void;
};

const PlayerListItem = ({
  player,
  centres,
  winner,
  isExpanded,
  toggleExpand,
}: PlayerListItemProps) => {
  const centreCount = centres.length;
  const isEliminated = centreCount === 0 || (winner && player !== winner);
  const colour = getNationColour(player);

  return (
    <>
      <div
        className="text-lg flex items-center"
        style={{
          opacity: isEliminated ? 0.3 : 1,
          color: colour,
        }}
      >
        <p className="min-w-20 text-start">{player}</p>
        <p className="min-w-16 font-bold text-end">
          {`${centreCount}/${victoryRequiredCentreCount}`}
        </p>
        <ExpandButton colour={colour} isExpanded={isExpanded} toggleExpand={toggleExpand} />
      </div>
      {isExpanded &&
        centres.map((centre) => (
          <p className="text-sm ml-2 -mt-1" style={{ color: colour }}>
            {centre}
          </p>
        ))}
    </>
  );
};

export default PlayerListItem;
