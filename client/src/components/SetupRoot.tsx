import { useState } from 'react';
import LandingPage from './pages/LandingPage';
import SetupViewOption from '../types/enums/setupViewOption';
import NewGamePage from './pages/NewGamePage';
import JoinGamePage from './pages/JoinGamePage';
import LoadGamePage from './pages/LoadGamePage';

const SetupRoot = () => {
  const [option, setOption] = useState(SetupViewOption.None);

  return (
    <div className="bg-no-repeat bg-cover bg-center bg-[url(landing-image-background.png)] dark:bg-none">
      {option === SetupViewOption.None && <LandingPage setViewOption={setOption} />}
      {option === SetupViewOption.New && <NewGamePage setViewOption={setOption} />}
      {option === SetupViewOption.Join && <JoinGamePage setViewOption={setOption} />}
      {option === SetupViewOption.Load && <LoadGamePage setViewOption={setOption} />}
    </div>
  );
};

export default SetupRoot;
