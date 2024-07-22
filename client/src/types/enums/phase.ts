enum Phase {
  Spring = 'Spring',
  Fall = 'Fall',
  Winter = 'Winter',
}

export const getNextPhase = (year: number, phase: Phase) => {
  const nextYear = phase === Phase.Winter ? year + 1 : year;
  const nextPhase = {
    [Phase.Spring]: Phase.Fall,
    [Phase.Fall]: Phase.Winter,
    [Phase.Winter]: Phase.Spring,
  }[phase];
  return { year: nextYear, phase: nextPhase };
};

export const getPhaseIndex = (phase: Phase) =>
  ({
    [Phase.Spring]: 1,
    [Phase.Fall]: 2,
    [Phase.Winter]: 3,
  })[phase];

export const getEarliestPhase = (phase1: Phase, phase2: Phase) =>
  getPhaseIndex(phase1) > getPhaseIndex(phase2) ? phase2 : phase1;

export const getLatestPhase = (phase1: Phase, phase2: Phase) =>
  getPhaseIndex(phase1) < getPhaseIndex(phase2) ? phase2 : phase1;

export default Phase;
