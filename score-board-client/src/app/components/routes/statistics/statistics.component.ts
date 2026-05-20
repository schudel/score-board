import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormControl} from '@angular/forms';
import {GameService} from '../../../services/rest/game.service';
import {Game} from '../../../models/game';
import {PlayerService} from '../../../services/rest/player.service';
import {Player} from '../../../models/player';
import {MatchService} from '../../../services/rest/match.service';
import {Match} from '../../../models/match';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit, OnDestroy {
  subscriptions = [];
  selected = new FormControl(0);
  players: Player[] = [];
  games: Game[] = [];
  matches: Match[] = [];

  constructor(private activeRoute: ActivatedRoute,
              private gameService: GameService,
              private playerService: PlayerService,
              private matchService: MatchService) { }

  ngOnInit() {
    this.subscriptions.push(this.activeRoute.queryParams.subscribe (
      params => {
        const tab = params.tab || '';
        if (!tab || tab === '' || tab === 'players') {
          this.selected.setValue(0);
        } else if (tab === 'games') {
          this.selected.setValue(1);
        } else if (tab === 'matches') {
          this.selected.setValue(2);
        }
      }
    ));
    this.subscriptions.push(this.playerService.getAll().subscribe(p => {
      if (p) {
        this.players = p;
      }
    }));
    this.subscriptions.push(this.gameService.getAll().subscribe(g => {
      if (g) {
        this.games = g;
      }
    }));
    this.subscriptions.push(this.matchService.getAll(true).subscribe(m => {
      if (m) {
        this.matches = m;
      }
    }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
