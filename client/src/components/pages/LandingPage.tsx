import SetupViewOption from '../../types/enums/setupViewOption';
import Button from '../user-interface/common/Button';
import Link from '../user-interface/common/Link';
import LinkGroup from '../user-interface/common/LinkGroup';

type LandingPageProps = {
  setViewOption: (option: SetupViewOption) => void;
};

const LandingPage = ({ setViewOption }: LandingPageProps) => (
  <div className="flex justify-center w-screen min-h-screen">
    <div className="flex flex-col gap-6 py-8">
      <div className="flex flex-col my-8">
        <p className="text-[72px] font-bold">5D Diplomacy</p>
        <p className="text-[36px] font-bold">With Multiverse Time Travel</p>
      </div>
      <div className="flex flex-row gap-4 mb-8">
        <Button text="New Game" minWidth={184} onClick={() => setViewOption(SetupViewOption.New)} />
        <Button
          text="Join Game"
          minWidth={184}
          onClick={() => setViewOption(SetupViewOption.Join)}
        />
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
            href: 'https://instructions.hasbro.com/en-us/instruction/avalon-hill-diplomacy-cooperative-strategy-board-game',
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
          {
            text: 'Title page art - Lady Razor',
            href: 'https://www.thediplomats.net/ladyrazor',
          },
        ]}
      />
      <Link
        text="Source code"
        href="https://github.com/Oliveriver/5d-diplomacy-with-multiverse-time-travel"
      />
    </div>
    <img
      alt="The Time Machinations"
      src="./landing-image-foreground.png"
      className="py-4 max-h-screen"
    />
  </div>
);

export default LandingPage;
