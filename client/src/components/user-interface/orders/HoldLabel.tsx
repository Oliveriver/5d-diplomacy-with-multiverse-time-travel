import Location from '../../../types/location';
import Unit from '../../../types/unit';
import UnitIcon from '../../world/boards/UnitIcon';

type HoldLabelProps = {
  unit: Unit | null;
  location: Location;
};

const HoldLabel = ({ unit, location }: HoldLabelProps) => {
  if (!unit) return null;

  return (
    <div className="flex flex-row gap-2 items-center">
      <UnitIcon unit={unit} variant="overlay" />
      <p>{location.region}</p>
    </div>
  );
};

export default HoldLabel;
