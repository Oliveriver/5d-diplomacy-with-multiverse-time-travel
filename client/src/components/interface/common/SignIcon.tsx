import colours from '../../../utils/colours';

type SignIconProps = {
  type: 'plus' | 'minus';
  colour: string;
};

const SignIcon = ({ type, colour }: SignIconProps) => (
  <div className="relative w-3.5 h-3.5 rounded-full" style={{ backgroundColor: colour }}>
    <div
      className="absolute h-0.5 w-2.5 top-1.5 left-0.5 rounded"
      style={{ backgroundColor: colours.iconForeground }}
    />
    {type === 'plus' && (
      <div
        className="absolute h-2.5 w-0.5 left-1.5 top-0.5 rounded"
        style={{ backgroundColor: colours.iconForeground }}
      />
    )}
  </div>
);

export default SignIcon;
