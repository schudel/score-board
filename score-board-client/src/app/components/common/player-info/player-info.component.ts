import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Player} from '../../../models/player';
import {Match} from '../../../models/match';
import {Rating} from '../../../models/rating';
import {Game} from '../../../models/game';
import {MatchHelperService} from '../../../services/common/match-helper.service';
import {RatingHelperService} from '../../../services/common/rating-helper.service';
import {GameHelperService} from '../../../services/common/game-helper.service';
import {DateHelper} from '../../../helpers/date-helper';
import {Constants} from '../../../constants/constants';

@Component({
  selector: 'app-player-info',
  templateUrl: './player-info.component.html',
  styleUrls: ['./player-info.component.scss']
})
export class PlayerInfoComponent implements OnInit, OnDestroy {
  subscriptions = [];
  ratingsInternal: Rating[];
  playerInternal: Player;
  currentRatings: Rating[];
  rating: Rating;
  games: Game[];
  gameId: string;

  constructor() {
    this.currentRatings = [];
    this.games = [];
  }

  @Input()
  set player(val: Player) {
    if (!val) {
      return;
    }
    this.playerInternal = val;
    this.updateCurrentRating(this.playerInternal);
  }
  @Input() matches: Match[];
  @Input() disableGameSelection: boolean;

  @Input()
  set ratings(val: Rating[]) {
    if (!val) {
      return;
    }
    this.ratingsInternal = val.sort(RatingHelperService.sortByConservativeRating);
    if (!this.ratingsInternal) {
      return;
    }
    for (const rating of this.ratingsInternal) {
      if (!GameHelperService.contains(rating.game, this.games)) {
        this.games.push(rating.game);
      }
    }
    if (this.games.length > 0) {
      this.changeGame(this.games[0].id);
    }
  }

  @Input()
  set game(val: Game) {
    if (!val) {
      return;
    }
    this.changeGame(val.id);
  }

  @Input() favoriteTeamMember: string;
  @Input() favoriteGame: string;

  ngOnInit() {
  }

  getTeamId(match: Match): string {
    if (match.game.id !== this.gameId) {
      return '';
    }
    return MatchHelperService.getTeamId(match, this.playerInternal);
  }

  getGamesCount(): number {
    if (!this.matches) {
      return 0;
    }
    let count = 0;
    for (const match of this.matches) {
      const teamId = this.getTeamId(match);
      if (teamId !== '') {
        count++;
      }
    }
    return count;
  }

  getWonGames(): string {
    if (!this.matches) {
      return '';
    }
    let count = 0;
    for (const match of this.matches) {
      const teamId = this.getTeamId(match);
      if (match.game.id === this.gameId && match.winnerTeamId && match.winnerTeamId === teamId) {
        count++;
      }
    }
    return count + ' (' + (count * 100 / this.getGamesCount()).toFixed(2) + '%)';
  }

  getLostGames(): string {
    if (!this.matches) {
      return '';
    }
    let count = 0;
    for (const match of this.matches) {
      const teamId = this.getTeamId(match);
      if (match.game.id === this.gameId && match.loserTeamId && match.loserTeamId === teamId) {
        count++;
      }
    }
    return count + ' (' + (count * 100 / this.getGamesCount()).toFixed(2) + '%)';
  }

  getDrawnGames(): string {
    if (!this.matches) {
      return '';
    }
    let count = 0;
    for (const match of this.matches) {
      if (match.game.id === this.gameId && match.drawn) {
        count++;
      }
    }
    return count + ' (' + (count * 100 / this.getGamesCount()).toFixed(2) + '%)';
  }

  changeGame($event: any) {
    this.gameId = $event;
    this.currentRatings = RatingHelperService.filterByGame($event, this.ratingsInternal);
    if (!this.playerInternal || !this.currentRatings) {
      return;
    }
    this.updateCurrentRating(this.playerInternal);
  }

  updateCurrentRating(p: Player) {
    if (!p) {
      return;
    }
    for (const rating of this.currentRatings) {
      if (rating.player.id === p.id) {
        this.rating = rating;
        return;
      }
    }
    this.rating = null;
  }

  getRanking(element: Rating): number {
    return RatingHelperService.getRanking(element.player, element.game, this.ratingsInternal);
  }

  getDate(date) {
    if (!date) {
      return '';
    }
    return DateHelper.GetDate(date.toString());
  }

  getDateFormat(): string {
    return Constants.DateFormat;
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
