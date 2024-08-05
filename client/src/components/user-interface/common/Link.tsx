type LinkProps = {
  text: string;
  href: string;
};

const Link = ({ text, href }: LinkProps) => (
  <a
    href={href}
    target="_blank"
    rel="noreferrer"
    className="text-blue-400 hover:text-blue-300 w-max"
  >
    {text}
  </a>
);

export default Link;
