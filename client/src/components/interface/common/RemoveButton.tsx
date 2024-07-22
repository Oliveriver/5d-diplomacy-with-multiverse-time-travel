import colours from '../../../utils/colours';

type RemoveButtonProps = {
  isDisabled: boolean;
  remove: () => void;
};

const RemoveButton = ({ isDisabled, remove }: RemoveButtonProps) => (
  <button
    type="button"
    onClick={remove}
    className="relative w-3.5 h-3.5 rounded-full ml-3 opacity-30 hover:opacity-100 cursor-pointer"
    style={{ backgroundColor: colours.iconDelete, pointerEvents: isDisabled ? 'none' : 'all' }}
    aria-label="Delete order"
    disabled={isDisabled}
  >
    <div
      className="absolute h-0.5 w-2.5 top-1.5 left-0.5 rounded rotate-45"
      style={{ backgroundColor: colours.iconForeground }}
    />
    <div
      className="absolute h-2.5 w-0.5 left-1.5 top-0.5 rounded rotate-45"
      style={{ backgroundColor: colours.iconForeground }}
    />
  </button>
);

export default RemoveButton;
