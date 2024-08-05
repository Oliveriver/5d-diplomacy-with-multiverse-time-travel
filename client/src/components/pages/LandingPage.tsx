import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../user-interface/common/Button';
import Link from '../user-interface/common/Link';
import LinkGroup from '../user-interface/common/LinkGroup';

type LandingPageProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const LandingPage = ({ setViewOption }: LandingPageProps) => (
  <div
    className="flex flex-col gap-6 w-full min-h-screen py-8 px-16"
    style={{
      // backgroundImage: TBA
      backgroundRepeat: 'no-repeat',
      backgroundSize: 'cover',
      backgroundPosition: 'center',
    }}
  >
    <div className="flex flex-col my-8">
      <p className="text-[72px] font-bold">5D Diplomacy</p>
      <p className="text-[36px] font-bold">With Multiverse Time Travel</p>
    </div>
    <div className="flex flex-row gap-4 mb-8">
      <Button text="New Game" minWidth={184} onClick={() => setViewOption(SetupViewOption.New)} />
      <Button text="Join Game" minWidth={184} onClick={() => setViewOption(SetupViewOption.Join)} />
    </div>
    <LinkGroup
      heading="Created by"
      links={[
        {
          text: 'Oliver Lugg',
          href: 'https://www.youtube.com/@OliverLugg',
        },
      ]}
    />
    <LinkGroup
      heading="Inspired by"
      links={[
        {
          text: 'Diplomacy',
          href: 'https://shop.hasbro.com/en-us/product/avalon-hill-diplomacy-cooperative-strategy-board-game-ages-12-and-up-2-7-players/09A402C7-4CA2-4E9D-9449-4592B2066011',
        },
        {
          text: '5D Chess With Multiverse Time Travel',
          href: 'https://www.5dchesswithmultiversetimetravel.com/',
        },
      ]}
    />
    <LinkGroup
      heading="Assets"
      links={[
        {
          text: 'Diplomacy map SVG - Martin Asal',
          href: 'https://commons.wikimedia.org/wiki/File:Diplomacy.svg',
        },
      ]}
    />
    <Link
      text="Source code"
      href="https://github.com/Oliveriver/5d-diplomacy-with-multiverse-time-travel"
    />
  </div>
);

export default LandingPage;
