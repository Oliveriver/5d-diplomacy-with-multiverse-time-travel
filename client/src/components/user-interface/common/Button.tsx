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
  return (
    <div className="relative">
      <button
        type="button"
        onClick={onClick}
        disabled={isDisabled || isBusy}
        className="rounded-xl border-4 pointer-events-auto flex items-center justify-center hover:!border-lime-500 disabled:opacity-30"
        style={{
          minWidth,
          minHeight,
          backgroundColor: colours.uiBackground,
          borderColor: isSelected ? colours.uiHighlight : colours.uiBorder,
        }}
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
