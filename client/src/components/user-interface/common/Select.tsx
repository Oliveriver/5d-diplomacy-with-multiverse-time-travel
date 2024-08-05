import colours from '../../../utils/colours';

type SelectProps<T extends string | number | undefined> = {
  options: {
    value: T;
    text: string;
    color?: string;
  }[];
  setValue: (value: T) => void;
  selectedValue: T;
};

const Select = <T extends string | number | undefined>({
  options,
  setValue,
  selectedValue,
}: SelectProps<T>) => (
  <div className="w-64 pr-4 bg-white border-4 rounded-xl">
    <select
      className="text-lg w-full bg-transparent p-4"
      style={{
        color: options.find((v) => v.value === selectedValue)?.color ?? colours.uiForeground,
      }}
      onChange={(event) => setValue(event.target.value as T)}
    >
      {options.map((option) => (
        <option
          key={option.text}
          value={option.value}
          className="text-lg"
          style={{ color: option.color ?? colours.uiForeground }}
        >
          {option.text}
        </option>
      ))}
    </select>
  </div>
);

export default Select;
