import { Injectable } from '@angular/core';
import {LiveMatch} from '../../models/live-match';
import {MatchState} from '../../models/match-state';

@Injectable({
  providedIn: 'root'
})
export class LiveMatchHelperService {

  constructor() { }

  public static processLiveMatch(liveMatches: LiveMatch[], liveMatch: LiveMatch): LiveMatch[] {
    if (liveMatches.length === 0 &&
      (liveMatch.state === MatchState.Playing.matchStateCode ||
        liveMatch.state === MatchState.Scheduled.matchStateCode ||
        liveMatch.state === MatchState.Interrupted.matchStateCode)) {
      liveMatches.push(liveMatch);
      return liveMatches;
    }
    let index = -1;
    let count = 0;
    let m;
    for (const match of liveMatches) {
      if (match.matchId.toLowerCase() === liveMatch.matchId.toLowerCase()) {
        match.score1 = liveMatch.score1;
        match.score2 = liveMatch.score2;
        match.state = liveMatch.state;
        match.timeStamp = liveMatch.timeStamp;
        index = count;
        m = match;
        break;
      }
      count++;
    }
    if (index === -1) {
      if (liveMatch.state === MatchState.Playing.matchStateCode ||
        liveMatch.state === MatchState.Scheduled.matchStateCode ||
        liveMatch.state === MatchState.Interrupted.matchStateCode) {
        liveMatches.push(liveMatch);
      }
    } else {
      if (liveMatch.state === MatchState.Done.matchStateCode ||
        liveMatch.state === MatchState.Aborted.matchStateCode) {
        liveMatches = liveMatches.filter(obj => obj !== m);
      }
    }
    return liveMatches;
  }
}
