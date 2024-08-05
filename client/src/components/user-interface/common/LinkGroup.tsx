import Link from './Link';

type LinkGroupProps = {
  heading: string;
  links: {
    text: string;
    href: string;
  }[];
};

const LinkGroup = ({ heading, links }: LinkGroupProps) => (
  <div className="flex flex-col gap-2">
    <p>{heading}</p>
    {links.map((link) => (
      <Link key={link.text} {...link} />
    ))}
  </div>
);

export default LinkGroup;
