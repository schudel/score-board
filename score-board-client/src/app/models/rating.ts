import {Player} from './player';
import {Game} from './game';

export class Rating {
  id: string;
  conservativeRating: number;
  mean: number;
  standardDeviation: number;
  player: Player;
  game: Game;
}
