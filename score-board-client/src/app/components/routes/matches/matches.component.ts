import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatchService} from '../../../services/rest/match.service';
import {Match} from '../../../models/match';

@Component({
  selector: 'app-matches',
  templateUrl: './matches.component.html',
  styleUrls: ['./matches.component.scss']
})
export class MatchesComponent implements OnInit, OnDestroy {
  subscriptions = [];
  matches: Match[];

  constructor(private matchService: MatchService) { }

  ngOnInit() {
    this.subscriptions.push(this.matchService.getAll().subscribe(
      m => {
        this.matches = m;
      }
    ));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
