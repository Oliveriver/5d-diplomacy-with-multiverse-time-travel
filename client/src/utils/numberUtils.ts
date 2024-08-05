/* eslint-disable import/prefer-default-export */

export const isInteger = (value: string, allowNegative: boolean = true) => {
  const parsedValue = parseInt(value, 10);

  // None of these cover every case (e.g. whitespace), so use all of them
  return (
    parsedValue.toString() === value &&
    !Number.isNaN(parsedValue) &&
    Number.isInteger(parsedValue) &&
    (allowNegative || parsedValue >= 0)
  );
};
