export class RatingHistory {
  id: string;
  dateTime?: Date;
  matchId?: string;
  gameId?: string;
  playerId?: string;
  playerName?: string;
  conservativeRating: number;
  mean: number;
  standardDeviation: number;
}
