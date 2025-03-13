import { SVGProps } from 'react';

const getSvgs = <T extends unknown>(input: Record<string, T>, id: string) => {
  let lookup = [id];
  if (id === 'Con')
    lookup = ['Con_']; // Can't name a file 'Con' on Windows...thanks Tom Scott...
  else if (id === 'Bul') lookup = ['Bul_E', 'Bul_S'];
  else if (id === 'Spa') lookup = ['Spa_N', 'Spa_S'];
  else if (id === 'Stp') lookup = ['Stp_N', 'Stp_S'];
  const svgs = lookup.map((l) => ({ svg: input[`../assets/map/${l}.svg`], key: l }));
  if (svgs.some((svg) => svg === undefined))
    throw new Error(`Failed region svg lookup for id ${id} (${lookup})`);

  return svgs;
};

const reactSvgs = import.meta.glob<
  React.FunctionComponent<React.ComponentProps<'svg'> & { title?: string }>
>('../assets/map/*.svg', {
  eager: true,
  query: '?react',
  import: 'default',
});

export const useRegionSvg = (id: string) => {
  const svgs = getSvgs(reactSvgs, id);

  if (svgs.length === 1) return svgs[0].svg;
  return (props: SVGProps<SVGSVGElement>) => (
    <>
      {svgs.map((x) => (
        <x.svg key={x.key} {...props} />
      ))}
    </>
  );
};

const rawSvgs = import.meta.glob<string>('../assets/map/*.svg', {
  eager: true,
  query: '?raw',
  import: 'default',
});

export const getRegionSvgRaw = (id: string) => getSvgs(rawSvgs, id).map((x) => x.svg);
