import colours from '../../../utils/colours';

type CheckboxProps = {
  title: string;
  value: boolean;
  onChange: (value: boolean) => void;
};

const Checkbox = ({ title, value, onChange }: CheckboxProps) => (
  <div className="flex flex-col items-center gap-2 ml-4">
    <p className="text-sm">{title}</p>
    <button
      type="button"
      aria-label={title}
      className="w-6 h-6 rounded border-4"
      style={{ backgroundColor: value ? colours.uiHighlight : colours.uiOverlay }}
      onClick={() => onChange(!value)}
    />
  </div>
);

export default Checkbox;
