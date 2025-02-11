import cn from 'classnames'
import { PropsWithChildren } from 'react';
import { Dots } from 'react-activity';
import css from './button.module.scss'

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
        className={cn(css.button, {[css.selected]: isSelected})}
        style={{ minWidth, minHeight }}
      >
        <p className={css.innerText}>{text}</p>
        {children}
      </button>
      {isBusy && (
        <div className={css.busy}>
          <Dots color="gray" size={32} speed={0.5} />
        </div>
      )}
    </div>
  );
};

export default Button;
