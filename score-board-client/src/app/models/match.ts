import {Game} from './game';
import {Team} from './team';

export class Match {
  id: string;
  game: Game;
  team1: Team;
  team2: Team;
  startTime?: Date;
  stopTime?: Date;
  duration?: string;
  score1: number;
  score2: number;
  state: string;
  drawn: boolean;
  winnerTeamId: string;
  loserTeamId: string;
  matchQuality?: number;
}
