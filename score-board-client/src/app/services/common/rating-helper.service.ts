import { Injectable } from '@angular/core';
import {Rating} from '../../models/rating';
import {Player} from '../../models/player';
import {Game} from '../../models/game';

@Injectable({
  providedIn: 'root'
})
export class RatingHelperService {

  constructor() { }

  static sortByMean(r1: Rating, r2: Rating): number {
    if (r1.mean > r2.mean) {
      return -1;
    }
    if (r1.mean < r2.mean) {
      return 1;
    }
    return 0;
  }

  static sortByConservativeRating(r1: Rating, r2: Rating): number {
    if (r1.conservativeRating > r2.conservativeRating) {
      return -1;
    }
    if (r1.conservativeRating < r2.conservativeRating) {
      return 1;
    }
    return 0;
  }

  static getRanking(player: Player, game: Game, ratings: Rating[]): number {
    if (!player || !ratings || !game) {
      return 0;
    }
    let count = 0;
    ratings = RatingHelperService.filterByGame(game.id, ratings);
    ratings = ratings.sort(RatingHelperService.sortByConservativeRating);
    for (const r of ratings) {
      if (r.player.id === player.id && game.id === r.game.id) {
        return count + 1;
      }
      count++;
    }
  }

  static filterByGame(gameId: string, ratings: Rating[]): Rating[] {
    if (!ratings || !gameId) {
      return;
    }
    const filteredRatings = [];
    for (const rating of ratings) {
      if (rating.game.id === gameId) {
        filteredRatings.push(rating);
      }
    }
    return filteredRatings;
  }
}
