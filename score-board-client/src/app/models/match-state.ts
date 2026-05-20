enum MatchStateEnum {
  Scheduled = 'Scheduled',
  Playing = 'Playing',
  Done = 'Done',
  Interrupted = 'Interrupted',
  Aborted = 'Aborted'
}

export class MatchState {
  private constructor(matchStateCode: string, matchStateEnum: MatchStateEnum, displayName: string) {
    this.matchStateCode = matchStateCode;
    this.matchStateEnum = matchStateEnum;
    this.displayName = displayName;
    MatchState.matchStates.push(this);
  }
  private static matchStates: Array<MatchState> = new Array<MatchState>();

  public static Scheduled = new MatchState('Scheduled', MatchStateEnum.Scheduled, 'Scheduled');
  public static Playing = new MatchState('Playing', MatchStateEnum.Playing, 'Playing');
  public static Done = new MatchState('Done', MatchStateEnum.Done, 'Done');
  public static Interrupted = new MatchState('Interrupted', MatchStateEnum.Interrupted, 'Interrupted');
  public static Aborted = new MatchState('Aborted', MatchStateEnum.Aborted, 'Aborted');

  matchStateCode: string;
  matchStateEnum: MatchStateEnum;
  displayName: string;

  static getMatchStates(): Array<MatchState> {
    return MatchState.matchStates;
  }

  static getMatchState(matchStateCode: string) {
    for (const matchState of MatchState.matchStates) {
      if (matchStateCode === matchState.matchStateCode) {
        return matchState;
      }
    }
    return null;
  }
}
