import { Injectable } from '@angular/core';
import {Match} from '../../models/match';
import {Player} from '../../models/player';

@Injectable({
  providedIn: 'root'
})
export class MatchHelperService {

  constructor() { }

  static sortMatchesByQuality(m1: Match, m2: Match): number {
    if (m1.matchQuality > m2.matchQuality) {
      return -1;
    }
    if (m1.matchQuality < m2.matchQuality) {
      return 1;
    }
    return 0;
  }

  public static getTeamId(match: Match, player: Player): string {
    let teamId = '';
    if (!player || !match) {
      return '';
    }
    if (match.team1 && match.team1.player1 && match.team1.player1.id === player.id ) {
      teamId = match.team1.id;
    }
    if (match.team1 && match.team1.player2 && match.team1.player2.id === player.id ) {
      teamId = match.team1.id;
    }
    if (match.team2 && match.team2.player1 && match.team2.player1.id === player.id) {
      teamId = match.team2.id;
    }
    if (match.team2 && match.team2.player2 && match.team2.player2.id === player.id ) {
      teamId = match.team2.id;
    }
    return teamId;
  }

  public static getTeamMember(match: Match, player: Player): string {
    if (!player || !match) {
      return '';
    }
    if (match.team1 && match.team1.player1 && match.team1.player1.id === player.id ) {
      if (match.team1.player2) {
        return match.team1.player2.playerName;
      }
    }
    if (match.team1 && match.team1.player2 && match.team1.player2.id === player.id ) {
      if (match.team1.player1) {
        return match.team1.player1.playerName;
      }
    }
    if (match.team2 && match.team2.player1 && match.team2.player1.id === player.id) {
      if (match.team2.player2) {
        return match.team2.player2.playerName;
      }
    }
    if (match.team2 && match.team2.player2 && match.team2.player2.id === player.id ) {
      if (match.team2.player1) {
        return match.team2.player1.playerName;
      }
    }
    return '';
  }

  public static getFavoriteGame(matches: Match[], player: Player): string {
    if (!matches || !player) {
      return '';
    }
    const map = new Map<string, number>();
    for (const match of matches) {
      if (MatchHelperService.getTeamId(match, player) === '') {
        continue;
      }
      const game = match.game.name;
      if (map.has(game)) {
        map.set(game, map.get(game) + 1);
      } else {
        map.set(game, 0);
      }
    }
    let highest = 0;
    let favorites = '';
    map.forEach((value: number, key: string) => {
      if (value > highest) {
        favorites = key;
        highest = value;
      } else if (value === highest) {
        if (favorites === '') {
          favorites = key;
        } else {
          favorites = favorites + ', ' + key;
        }
      }
    });
    return favorites;
  }

  public static getFavoriteTeamMember(matches: Match[], player: Player): string {
    if (!matches || !player) {
      return '';
    }
    const map = new Map<string, number>();
    for (const match of matches) {
      const teamMember = MatchHelperService.getTeamMember(match, player);
      if (teamMember === '') {
        continue;
      }
      if (map.has(teamMember)) {
        map.set(teamMember, map.get(teamMember) + 1);
      } else {
        map.set(teamMember, 0);
      }
    }
    let highest = 0;
    let favorites = '';
    map.forEach((value: number, key: string) => {
      if (value > highest) {
        favorites = key;
        highest = value;
      } else if (value === highest) {
        if (favorites === '') {
          favorites = key;
        } else {
          favorites = favorites + ', ' + key;
        }
      }
    });
    return favorites;
  }

  public static allowedToEdit(match: Match, player: Player): boolean {
    if (!match) {
      return false;
    }
    if (player.id === match.team1.player1.id) {
      return true;
    }
    if (match.team1.player2 && player.id === match.team1.player2.id) {
      return true;
    }
    if (player.id === match.team2.player1.id) {
      return true;
    }
    return match.team2.player2 && player.id === match.team2.player2.id;
  }
}
