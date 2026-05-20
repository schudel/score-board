import { Injectable } from '@angular/core';
import {Game} from '../../models/game';

@Injectable({
  providedIn: 'root'
})
export class GameHelperService {

  constructor() { }

  static contains(g: Game, gs: Game[]): boolean {
    if (!g || !gs) {
      return false;
    }
    for (const game of gs) {
      if (g.id === game.id) {
        return true;
      }
    }
    return false;
  }
}
