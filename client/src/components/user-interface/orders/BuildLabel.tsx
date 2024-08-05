import { getNationColour } from '../../../types/enums/nation';
import Location from '../../../types/location';
import Unit from '../../../types/unit';
import UnitIcon from '../../world/boards/UnitIcon';
import SignIcon from '../common/SignIcon';

type BuildLabelProps = {
  unit: Unit | null;
  location: Location;
};

const BuildLabel = ({ unit, location }: BuildLabelProps) => {
  if (!unit) return null;

  return (
    <div className="flex flex-row gap-2 items-center">
      <SignIcon type="plus" colour={getNationColour(unit.owner)} />
      <UnitIcon unit={unit} variant="overlay" />
      <p>{location.region}</p>
    </div>
  );
};

export default BuildLabel;
