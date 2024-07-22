import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../interface/common/Button';
import Link from '../interface/common/Link';
import LinkGroup from '../interface/common/LinkGroup';

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
          href: 'https://boardgamegeek.com/boardgame/483/diplomacy',
        },
        {
          text: '5D Chess With Multiverse Time Travel',
          href: 'https://store.steampowered.com/app/1349230/5D_Chess_With_Multiverse_Time_Travel/',
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
    <Link text="Source code" href="TBA" />
  </div>
);

export default LandingPage;
