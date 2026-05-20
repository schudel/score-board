import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {PlayerService} from '../../../services/rest/player.service';
import {Player} from '../../../models/player';
import {MatchService} from '../../../services/rest/match.service';
import {Match} from '../../../models/match';
import {RatingService} from '../../../services/rest/rating.service';
import {Rating} from '../../../models/rating';
import {Game} from '../../../models/game';
import {MatchHelperService} from '../../../services/common/match-helper.service';
import {GameHelperService} from '../../../services/common/game-helper.service';
import {animate, keyframes, query, stagger, style, transition, trigger} from '@angular/animations';
import {RatingHelperService} from '../../../services/common/rating-helper.service';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss'],
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
export class PlayerComponent implements OnInit, OnDestroy {
  subscriptions = [];
  player: Player;
  matches: Match[];
  ratings: Rating[];
  games: Game[];
  private playerId: string;

  constructor(private activeRoute: ActivatedRoute,
              private playerService: PlayerService,
              private matchService: MatchService,
              private ratingService: RatingService) {
    this.games = [];
  }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    if (routeParams == null || routeParams.id == null) {
      return;
    }
    this.playerId = routeParams.id;
    if (this.playerId) {
      this.subscriptions.push(this.playerService.get(this.playerId).subscribe(p => {
        this.player = p;
        this.sortGamesByRanking(this.ratings, this.games, this.player.id);
      }));
      this.subscriptions.push(this.matchService.getByPlayer(this.playerId, true).subscribe(m => { this.matches = m; }));
    }
    this.subscriptions.push(this.ratingService.getAll().subscribe(r => {
      this.ratings = r;
      for (const rating of r) {
        if (!GameHelperService.contains(rating.game, this.games)) {
          this.games.push(rating.game);
        }
      }
      if (this.player) {
        this.sortGamesByRanking(this.ratings, this.games, this.player.id);
      } else {
        this.sortGamesByRanking(this.ratings, this.games, null);
      }
    }));
  }

  sortGamesByRanking(r: Rating[], games: Game[], playerId: string): void {
    this.games = [];
    if (!r || !games || !playerId) {
      return;
    }
    r = r.sort(RatingHelperService.sortByConservativeRating);
    for (const rating of r) {
      if (rating.player.id === playerId && !GameHelperService.contains(rating.game, this.games)) {
        this.games.push(rating.game);
      }
    }
  }

  getFavoriteGame(): string {
    return MatchHelperService.getFavoriteGame(this.matches, this.player);
  }

  getFavoriteTeamMember(): string {
    return MatchHelperService.getFavoriteTeamMember(this.matches, this.player);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
