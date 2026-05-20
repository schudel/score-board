import {Component, OnDestroy, OnInit} from '@angular/core';
import {PlayerService} from '../../../services/rest/player.service';
import {Player} from '../../../models/player';
import {Match} from '../../../models/match';
import {MatchService} from '../../../services/rest/match.service';
import {RatingService} from '../../../services/rest/rating.service';
import {Rating} from '../../../models/rating';
import {Game} from '../../../models/game';
import {MatchHelperService} from '../../../services/common/match-helper.service';
import {GameHelperService} from '../../../services/common/game-helper.service';
import {RatingHelperService} from '../../../services/common/rating-helper.service';
import {animate, keyframes, query, stagger, style, transition, trigger} from '@angular/animations';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.scss'],
  animations: [
    // Trigger animation cards array
    trigger('cardAnimation', [
      // Transition from any state to any state
      transition('* => *', [
        // Initially the all cards are not visible
        query(':enter', style({ opacity: 0 }), { optional: true }),
        // Each card will appear sequentially with the delay of 100ms
        query(':enter', stagger('75ms', [
          animate('.5s ease-in', keyframes([
            style({ opacity: 0, transform: 'translateY(-50%)', offset: 0 }),
            style({ opacity: .5, transform: 'translateY(-10px) scale(1.1)', offset: 0.3 }),
            style({ opacity: 1, transform: 'translateY(0)', offset: 1 }),
          ]))]), { optional: true }),
        // Cards will disappear sequentially with the delay of 100ms
        query(':leave', stagger('75ms', [
          animate('300ms ease-out', keyframes([
            style({ opacity: 1, transform: 'scale(1.1)', offset: 0 }),
            style({ opacity: .5, transform: 'scale(.5)', offset: 0.3 }),
            style({ opacity: 0, transform: 'scale(0)', offset: 1 }),
          ]))]), { optional: true })
      ])
    ]),
    // Trigger animation for plus button
    trigger('plusAnimation', [
      // Transition from any state to any state
      transition('* => *', [
        query('.plus-card', style({ opacity: 0, transform: 'translateY(-40px)' }), { optional: true }),
        query('.plus-card', stagger('300ms', [
          animate('100ms 1.1s ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
        ]), { optional: true })
      ])
    ])
  ]
})
export class PlayersComponent implements OnInit, OnDestroy {
  subscriptions = [];
  allPlayers: Player[];
  players: Player[];
  matches: Match[];
  ratings: Rating[];
  game: Game;
  games: Game[];
  gameId: string;

  constructor(private playerService: PlayerService,
              private matchService: MatchService,
              private ratingService: RatingService) {
    this.games = [];
  }

  ngOnInit() {
    this.subscriptions.push(this.playerService.getAll().subscribe(p => {
      this.allPlayers = p;
      this.sortPlayerByRanking(this.ratings, this.allPlayers, this.gameId);
    }));
    this.subscriptions.push(this.matchService.getAll().subscribe(m => this.matches = m));
    this.subscriptions.push(this.ratingService.getAll().subscribe(r => {
      this.ratings = r;
      if (!this.ratings) {
        return;
      }
      for (const rating of this.ratings) {
        if (!GameHelperService.contains(rating.game, this.games)) {
          this.games.push(rating.game);
        }
      }
      if (this.games.length > 0) {
        this.game = this.games[0];
      }
      if (this.game && this.game.id) {
        this.changeGame(this.game.id);
      }
    }));
  }

  getMatches(playerId): Match[] {
    if (!this.matches) {
      return [];
    }
    const m: Match[] = [];
    for (const match of this.matches) {
      if ((match.team1.player1 && match.team1.player1.id === playerId) ||
        (match.team1.player2 && match.team1.player2.id === playerId) ||
        (match.team2.player1 && match.team2.player1.id === playerId) ||
        (match.team2.player2 && match.team2.player2.id === playerId) ) {
        m.push(match);
      }
    }
    return m;
  }

  getFavoriteGame(player: Player): string {
    return MatchHelperService.getFavoriteGame(this.matches, player);
  }

  getFavoriteTeamMember(player: Player): string {
    return MatchHelperService.getFavoriteTeamMember(this.matches, player);
  }

  changeGame($event: any) {
    this.gameId = $event;
    for (const g of this.games) {
      if (this.gameId === g.id) {
        this.game = g;
        this.sortPlayerByRanking(this.ratings, this.allPlayers, this.gameId);
        return;
      }
    }
  }

  sortPlayerByRanking(r: Rating[], p: Player[], gameId: string): void {
    const firstTime = (this.players && this.players.length === 0);
    this.players = [];
    if (!r || !p || !gameId) {
      return;
    }
    let timeout = 0;
    if (!firstTime) {
      timeout = this.allPlayers.length * 75;
    }
    setTimeout( () => {
      r = r.sort(RatingHelperService.sortByConservativeRating);
      for (const rating of r) {
        if (rating.game.id === gameId && !this.contains(rating.player, this.players)) {
          this.players.push(this.getPlayerFromList(p, rating.player.id));
        }
      }
      for (const player of p) {
        if (!this.contains(player, this.players)) {
          this.players.push(player);
        }
      }
    }, timeout);
  }

  contains(p: Player, pl: Player[]): boolean {
    if (!p || !pl) {
      return false;
    }
    for (const player of pl) {
      if (p.id === player.id) {
        return true;
      }
    }
    return false;
  }

  getPlayerFromList(pl: Player[], playerId: string): Player {
    for (const player of pl) {
      if (playerId === player.id) {
        return player;
      }
    }
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
