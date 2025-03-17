import Nation, { getNationColour } from '../../types/enums/nation';
import { victoryRequiredCentreCount } from '../../utils/constants';
import ExpandButton from './common/ExpandButton';
import colours from '../../utils/colours';

type PlayerListItemProps = {
  player: Nation;
  centres: string[];
  showSubmissionIndicator: boolean;
  hasSubmitted: boolean;
  winner: Nation | null;
  isExpanded: boolean;
  toggleExpand: () => void;
};

const PlayerListItem = ({
  player,
  centres,
  showSubmissionIndicator,
  hasSubmitted,
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
        {showSubmissionIndicator && (
          <div
            title={hasSubmitted ? 'Submitted' : 'Not submitted'}
            className="w-4 h-4 rounded-full mr-4 border-2"
            style={{
              backgroundColor: hasSubmitted && !winner ? colours.uiHighlight : colours.uiBorder,
              borderColor: colours.uiBorder,
            }}
          />
        )}
        <p className="min-w-20 text-start">{player}</p>
        <p className="min-w-16 font-bold text-end">
          {`${centreCount}/${victoryRequiredCentreCount}`}
        </p>
        <ExpandButton colour={colour} isExpanded={isExpanded} toggleExpand={toggleExpand} />
      </div>
      {isExpanded &&
        centres.map((centre) => (
          <p
            className="text-sm -mt-1"
            style={{ color: colour, marginLeft: showSubmissionIndicator ? 32 : 0 }}
          >
            {centre}
          </p>
        ))}
    </>
  );
};

export default PlayerListItem;
