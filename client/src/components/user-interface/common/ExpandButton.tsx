import colours from '../../../utils/colours';

type ExpandButtonProps = {
  colour: string;
  isExpanded: boolean;
  toggleExpand: () => void;
};

const ExpandButton = ({ colour, isExpanded, toggleExpand }: ExpandButtonProps) => (
  <button
    type="button"
    onClick={toggleExpand}
    className="relative w-3.5 h-3.5 rounded-full ml-4 opacity-30 hover:opacity-100 cursor-pointer"
    style={{ backgroundColor: colour }}
    aria-label="Expand item"
  >
    <div
      className={`absolute h-0.5 w-[5px] top-1.5 left-[3px] rounded ${isExpanded ? '-rotate-45' : 'rotate-45'}`}
      style={{ backgroundColor: colours.iconForeground }}
    />
    <div
      className={`absolute h-0.5 w-[5px] top-1.5 right-[3px] rounded ${isExpanded ? 'rotate-45' : '-rotate-45'}`}
      style={{ backgroundColor: colours.iconForeground }}
    />
  </button>
);

export default ExpandButton;
