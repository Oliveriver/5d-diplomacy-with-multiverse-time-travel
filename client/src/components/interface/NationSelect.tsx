import Nation, { getNationColour } from '../../types/enums/nation';
import Select from './common/Select';

type NationSelectProps = {
  selectedNation: Nation | undefined;
  setSelectedNation: (nation: Nation | undefined) => void;
};

const NationSelect = ({ selectedNation, setSelectedNation }: NationSelectProps) => (
  <Select
    options={[undefined, ...Object.values(Nation)].map((nation) => ({
      text: nation ?? 'Random',
      color: nation && getNationColour(nation),
      value: nation,
    }))}
    setValue={setSelectedNation}
    selectedValue={selectedNation}
  />
);

export default NationSelect;
