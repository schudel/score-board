import {Component, Input, OnInit} from '@angular/core';
import {Game} from '../../../models/game';
import {Rating} from '../../../models/rating';
import {RatingHelperService} from '../../../services/common/rating-helper.service';
import {GameHelperService} from '../../../services/common/game-helper.service';
import {Player} from '../../../models/player';

@Component({
  selector: 'app-number-one-widget',
  templateUrl: './number-one-widget.component.html',
  styleUrls: ['./number-one-widget.component.scss']
})
export class NumberOneWidgetComponent implements OnInit {
  ratingsInternal: Rating[];
  currentRatings: Rating[];
  rating: Rating;
  games: Game[];
  gameId: string;
  playersInternal: Player[];
  playerImage: string;

  constructor() {
    this.currentRatings = [];
    this.games = [];
  }

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
      this.game = this.games[0];
    }
    if (!this.gameId) {
      return;
    }
    this.currentRatings = RatingHelperService.filterByGame(this.gameId, this.ratingsInternal);
    this.rating = this.currentRatings[0];
    this.getPlayerImage(this.playersInternal);
  }

  @Input()
  set game(val: Game) {
    this.gameId = val.id;
  }

  @Input()
  set players(val: Player[]) {
    this.playersInternal = val;
    this.getPlayerImage(val);
  }

  ngOnInit() {
  }

  changeGame($event: any) {
    this.currentRatings = RatingHelperService.filterByGame($event, this.ratingsInternal);
    this.rating = this.currentRatings[0];
    this.getPlayerImage(this.playersInternal);
  }

  private getPlayerImage(players: Player[]) {
    if (!players || !this.rating) {
      return;
    }
    for (const player of players) {
      if (player.id === this.rating.player.id) {
        this.playerImage = player.image;
        break;
      }
    }
  }
}
