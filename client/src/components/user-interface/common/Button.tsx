import { PropsWithChildren } from 'react';
import { Dots } from 'react-activity';
import colours from '../../../utils/colours';

type ButtonProps = PropsWithChildren & {
  text?: string;
  onClick?: (() => void) | (() => Promise<void>);
  isSelected?: boolean;
  isDisabled?: boolean;
  isBusy?: boolean;
  minWidth?: number;
  minHeight?: number;
};

const Button = ({
  children,
  text,
  onClick,
  isSelected,
  isDisabled,
  isBusy,
  minWidth = 64,
  minHeight = 64,
}: ButtonProps) => {
  const baseStyle =
    'rounded-xl border-4 pointer-events-auto flex items-center justify-center';
  const hoverStyle = 'hover:border-lime-500';
  const selectedSyle = isSelected ? 'border-lime-500' : 'border-gray-200';
  const disabledSyle = 'disabled:opacity-30 disabled:border-gray-200';

  return (
    <div className="relative">
      <button
        type="button"
        onClick={onClick}
        disabled={isDisabled || isBusy}
        className={`${baseStyle} ${hoverStyle} ${disabledSyle} ${selectedSyle}`}
        style={{ minWidth, minHeight, backgroundColor: colours.uiBackground }}
      >
        <p className="text-xl font-bold">{text}</p>
        {children}
      </button>
      {isBusy && (
        <div className="absolute bottom-1 flex items-center justify-center w-full h-full">
          <Dots color="gray" size={32} speed={0.5} />
        </div>
      )}
    </div>
  );
};

export default Button;
