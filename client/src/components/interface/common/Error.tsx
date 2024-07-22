import { AxiosError } from 'axios';
import colours from '../../../utils/colours';

type ErrorProps = {
  error: Error;
};

const Error = ({ error }: ErrorProps) => {
  const baseMessage = `${error?.name}: ${error?.message}`;
  const serverMessage = (error as AxiosError)?.response?.data as string | undefined;
  return (
    <div className="flex flex-col items-center w-1/2 text-wrap" style={{ color: colours.uiError }}>
      <p>{baseMessage}</p>
      {serverMessage && <p>{serverMessage}</p>}
    </div>
  );
};

export default Error;
