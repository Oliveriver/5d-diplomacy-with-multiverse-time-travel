import cn from 'classnames'
import colours from '../../../utils/colours';
import css from './checkbox.module.scss'

type CheckboxProps = {
  title: string;
  value: boolean;
  onChange: (value: boolean) => void;
};

const Checkbox = ({ title, value, onChange }: CheckboxProps) => (
  <div className={css.container}>
    <span>{title}</span>
    <button
      type="button"
      aria-label={title}
      className={cn(css.checker, {[css.selected]: value})}
      style={{ backgroundColor: value ? colours.uiHighlight : colours.uiOverlay }}
      onClick={() => onChange(!value)}
    />
  </div>
);

export default Checkbox;
