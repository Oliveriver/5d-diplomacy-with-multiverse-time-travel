/* eslint-disable import/prefer-default-export */

export const filterUnique = <T>(list: T[]) =>
  list.filter(
    (a, index) =>
      index ===
      list.findIndex((b) => {
        if (typeof a !== 'object' || a === null) return a === b;

        return Object.keys(a).every((key) => a[key as keyof T] === b[key as keyof T]);
      }),
  );
