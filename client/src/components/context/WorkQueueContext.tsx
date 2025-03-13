import { createContext } from 'react';

const noopQueue: WorkQueue = { push: () => {}, sharedCache: new Map() };
const WorkQueueContext = createContext<WorkQueue>(noopQueue);

export const createWorkQueue = (): WorkQueue => {
  const queue: { work: () => void; rank: () => number }[] = [];
  const executeNextWorkItem = () => {
    const nextWorkItem = queue
      .map((item, index) => ({ work: item.work, rank: item.rank(), index }))
      .reduce((previous, current) => (current.rank < previous.rank ? current : previous));
    queue.splice(nextWorkItem.index, 1);
    nextWorkItem.work();
  };
  return {
    push: (workItem: (onComplete: () => void) => void, rank: () => number) => {
      queue.push({
        work: () => {
          workItem(() => {
            if (queue.length > 0) {
              setTimeout(executeNextWorkItem, 1);
            }
          });
        },
        rank,
      });

      if (queue.length === 1) {
        setTimeout(executeNextWorkItem, 1);
      }
    },
    sharedCache: new Map(),
  };
};

export type WorkQueue = {
  push: (workItem: (onComplete: () => void) => void, rank: () => number) => void;
  sharedCache: Map<unknown, unknown>;
};

export default WorkQueueContext;
