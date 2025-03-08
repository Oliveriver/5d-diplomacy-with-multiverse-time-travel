import { createContext } from 'react';

const noopQueue = { push: () => {} };
const WorkQueueContext = createContext<WorkQueue>(noopQueue);

export const createWorkQueue = () => {
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
  };
};

export type WorkQueue = {
  push: (workItem: (onComplete: () => void) => void, rank: () => number) => void;
};

export default WorkQueueContext;
