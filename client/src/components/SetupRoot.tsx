import { useState } from 'react';
import LandingPage from './pages/LandingPage';
import SetupViewOption from '../types/enums/setupViewOption';
import NewGamePage from './pages/NewGamePage';
import JoinGamePage from './pages/JoinGamePage';

const SetupRoot = () => {
  const [option, setOption] = useState(SetupViewOption.None);

  return (
    <div className="bg-no-repeat bg-cover bg-center bg-[url(landing-image-background.png)] dark:bg-none dark:bg-slate-950">
      {option === SetupViewOption.None && <LandingPage setViewOption={setOption} />}
      {option === SetupViewOption.New && <NewGamePage setViewOption={setOption} />}
      {option === SetupViewOption.Join && <JoinGamePage setViewOption={setOption} />}
    </div>
  );
};

export default SetupRoot;
