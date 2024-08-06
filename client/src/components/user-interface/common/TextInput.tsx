type TextInputProps = {
  placeholder: string;
  onChange: (value: string) => void;
};

const TextInput = ({ placeholder, onChange }: TextInputProps) => (
  <input
    type="text"
    className="w-64 h-16 p-4 border-4 rounded-xl text-lg"
    placeholder={placeholder}
    onChange={(event) => onChange(event.target.value)}
  />
);

export default TextInput;