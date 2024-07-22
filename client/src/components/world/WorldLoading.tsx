import { Dots } from 'react-activity';
import 'react-activity/dist/Dots.css';

const WorldLoading = () => (
  <div className="flex items-center justify-center w-screen h-screen">
    <Dots color="gray" size={64} speed={0.5} />
  </div>
);

export default WorldLoading;
