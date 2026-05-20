import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {GameService} from '../../../services/rest/game.service';
import {PlayerService} from '../../../services/rest/player.service';
import {MatchService} from '../../../services/rest/match.service';

@Component({
  selector: 'app-score-board-info',
  templateUrl: './score-board-info.component.html',
  styleUrls: ['./score-board-info.component.scss']
})
export class ScoreBoardInfoComponent implements OnInit, OnDestroy {
  subscriptions = [];
  gameCount: number;
  playerCount: number;
  matchCount: number;

  @Input() highestScore: number;

  constructor(private gameService: GameService, private playerService: PlayerService, private matchService: MatchService) {
    this.gameCount = 0;
    this.playerCount = 0;
    this.matchCount = 0;
  }

  ngOnInit() {
    this.subscriptions.push(this.gameService.count().subscribe(
      c => {
        this.gameCount = c;
      }
    ));
    this.subscriptions.push(this.playerService.count().subscribe(
      c => {
        this.playerCount = c;
      }
    ));
    this.subscriptions.push(this.matchService.count().subscribe(
      c => {
        this.matchCount = c;
      }
    ));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
