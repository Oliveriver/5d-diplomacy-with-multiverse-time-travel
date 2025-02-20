import colours from '../../../utils/colours';

type SelectProps<T extends string | number | undefined> = {
  options: {
    value: T;
    text: string;
    color?: string;
    hint?: string;
  }[];
  setValue: (value: T) => void;
  selectedValue: T;
};

const Select = <T extends string | number | undefined>({
  options,
  setValue,
  selectedValue,
}: SelectProps<T>) => {
  const selectedDetails = options.find((v) => v.value === selectedValue);

  return (
    <div
      className="w-64 pr-4 border-4 rounded-xl"
      style={{
        backgroundColor: colours.uiPageBackground,
        borderColor: colours.uiBorder,
      }}
    >
      <select
        className="text-lg w-full bg-transparent p-4"
        style={{ color: selectedDetails?.color ?? colours.uiForeground }}
        value={selectedValue}
        onChange={(event) => setValue(event.target.value as T)}
        title={selectedDetails?.hint}
      >
        {options.map((option) => (
          <option
            key={option.text}
            value={option.value}
            className="text-lg"
            style={{ color: option.color ?? colours.uiForeground }}
            title={option.hint}
          >
            {option.text}
          </option>
        ))}
      </select>
    </div>
  );
};

export default Select;
