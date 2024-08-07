import { useState } from 'react';
import Board, { getBoardName } from '../../../types/board';
import Phase from '../../../types/enums/phase';
import colours from '../../../utils/colours';
import { boardBorderWidth, boardSeparation, majorBoardWidth } from '../../../utils/constants';
import Map from './Map';
import TextInput from '../../user-interface/common/TextInput';
import Select from '../../user-interface/common/Select';
import { isInteger } from '../../../utils/numberUtils';
import Checkbox from '../../user-interface/common/Checkbox';

type LocationInputProps = {
  board: Board;
  onTimelineEntered: (value: string) => void;
  onYearEntered: (value: string) => void;
  onPhaseEntered: (value: Phase) => void;
  isShowingCoasts: boolean;
  onCoastsToggled: (value: boolean) => void;
};

const LocationInput = ({
  board,
  onTimelineEntered,
  onYearEntered,
  onPhaseEntered,
  isShowingCoasts,
  onCoastsToggled,
}: LocationInputProps) => (
  <div className="absolute -mt-20 flex flex-row gap-4 w-full justify-center items-center">
    <TextInput placeholder="Timeline" onChange={onTimelineEntered} />
    <Select
      options={[
        {
          text: 'Spring',
          value: Phase.Spring,
        },
        {
          text: 'Fall',
          value: Phase.Fall,
        },
      ]}
      selectedValue={board.phase}
      setValue={onPhaseEntered}
    />
    <TextInput placeholder="Year" onChange={onYearEntered} />
    <Checkbox title="Show coasts" value={isShowingCoasts} onChange={onCoastsToggled} />
  </div>
);

type BoardGhostProps = {
  initialTimeline: number;
  initialYear: number;
  initialPhase: Phase;
};

const BoardGhost = ({ initialTimeline, initialYear, initialPhase }: BoardGhostProps) => {
  const [timeline, setTimeline] = useState(initialTimeline);
  const [year, setYear] = useState(initialYear);
  const [phase, setPhase] = useState(initialPhase);
  const [isShowingCoasts, setIsShowingCoasts] = useState(false);

  const board = {
    timeline,
    year,
    phase,
    childTimelines: [],
    centres: {},
    units: {},
  };

  const onTimelineEntered = (value: string) => {
    if (!isInteger(value, false)) {
      setTimeline(initialTimeline);
      return;
    }

    const parsedValue = parseInt(value, 10);
    setTimeline(parsedValue);
  };

  const onYearEntered = (value: string) => {
    if (!isInteger(value, false)) {
      setYear(initialYear);
      return;
    }

    const parsedValue = parseInt(value, 10);
    setYear(parsedValue);
  };

  return (
    <div
      className="flex-col content-center relative"
      style={{
        minHeight: majorBoardWidth,
        height: majorBoardWidth,
        margin: boardSeparation / 2,
      }}
    >
      <LocationInput
        board={board}
        onTimelineEntered={onTimelineEntered}
        onYearEntered={onYearEntered}
        onPhaseEntered={setPhase}
        isShowingCoasts={isShowingCoasts}
        onCoastsToggled={setIsShowingCoasts}
      />
      <div
        className="relative rounded-xl"
        style={{
          backgroundColor: colours.boardBackground,
          minWidth: majorBoardWidth,
          width: majorBoardWidth,
          minHeight: majorBoardWidth,
          height: majorBoardWidth,
          borderWidth: boardBorderWidth,
          borderColor: colours.boardBorder,
        }}
      >
        <p className="text-md -mt-7">{getBoardName(board)}</p>
        <Map board={board} isShowingCoasts={isShowingCoasts} />
      </div>
    </div>
  );
};

export default BoardGhost;
