import colours from '../../utils/colours';
import Button from '../user-interface/common/Button';
import Error from '../user-interface/common/Error';

type WorldErrorProps = {
  error: Error;
  isLoading: boolean;
  retry: () => unknown;
};

const WorldError = ({ error, isLoading, retry }: WorldErrorProps) => (
  <div className="flex flex-col gap-10 items-center justify-center w-screen h-screen">
    <p className="text-[40px]" style={{ color: colours.uiError }}>
      Error fetching game state data
    </p>
    {error && <Error error={error} />}
    <Button text="Retry" onClick={retry} minWidth={96} isBusy={isLoading} />
  </div>
);

export default WorldError;
