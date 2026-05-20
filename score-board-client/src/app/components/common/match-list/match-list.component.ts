import {Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Match} from '../../../models/match';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {DateHelper} from '../../../helpers/date-helper';
import {Game} from '../../../models/game';
import {Player} from '../../../models/player';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Role} from '../../../models/role';
import {MatchHelperService} from '../../../services/common/match-helper.service';
import {Constants} from '../../../constants/constants';

@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.scss']
})
export class MatchListComponent implements OnInit, OnDestroy {
  subscriptions = [];
  currentPlayer: Player;
  dataSource = new MatTableDataSource<Match>();
  displayedColumns: string[] = ['Game', 'Team1', 'Team2', 'Score1', 'Score2', 'State', 'MatchQuality', 'StartTime', 'StopTime', 'Duration', 'Details', 'Edit'];
  matchesInternal: Match[];

  @ViewChild(MatSort, {static: true}) sort: MatSort;

  constructor(private authenticationService: AuthenticationService) { }

  @Input() sortColumn: string;
  @Input() title: string;
  @Input()
  set matches(val: Match[]) {
    this.matchesInternal = val;
    this.initDataSource(this.matchesInternal);
  }
  @Input() showCreateButton: boolean;
  @Input() showEditButton: boolean;

  ngOnInit() {
    if (!this.title) {
      this.title = 'Match List';
    }
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
    if (this.matchesInternal) {
      this.initDataSource(this.matchesInternal);
    }
  }

  initDataSource(matches: Match[]) {
    this.dataSource = new MatTableDataSource<Match>(matches);
    this.dataSource.sortingDataAccessor = (item: Match, property) => {
      switch (property) {
        case 'StartTime':
          return DateHelper.GetDate(item.startTime.toString()).getTime();
        case 'StopTime':
          return DateHelper.GetDate(item.stopTime.toString()).getTime();
        case 'Game':
          return item.game.name;
        case 'Team1':
          return item.team1.name;
        case 'Team2':
          return item.team2.name;
        default:
          return item[property];
      }
    };
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = this.createFilter();
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  getDate(date) {
    if (!date) {
      return '';
    }
    return DateHelper.GetDate(date.toString());
  }

  getDuration(duration) {
    if (!duration) {
      return '';
    }
    let split = duration.split('.');
    if (split.length === 2) {
      return split[0];
    } else {
      split = duration.split(',');
    }
    return split[0];
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  canUpdate(match: Match): boolean {
    return MatchHelperService.allowedToEdit(match, this.currentPlayer);
  }

  getDateFormat(): string {
    return Constants.DateFormat;
  }

  getTranslatedState(state: string) {
    return state;
    // TODO: fix localization
    // return $localize`:@@${state}:${state}`;
  }

  createFilter(): (item: Match, filter: string) => boolean {
    return (item, filter) => {
      return item.game.name.toLowerCase().indexOf(filter) !== -1 ||
        item.team1.name.toLowerCase().indexOf(filter) !== -1 ||
        item.team2.name.toLowerCase().indexOf(filter) !== -1 ||
        item.team1.player1.playerName.toLowerCase().indexOf(filter) !== -1 ||
        item.team2.player1.playerName.toLowerCase().indexOf(filter) !== -1 ||
        (item.team1.player2 && item.team1.player2.playerName.toLowerCase().indexOf(filter) !== -1) ||
        (item.team2.player2 && item.team2.player2.playerName.toLowerCase().indexOf(filter) !== -1) ||
        item.state.toLowerCase().indexOf(filter) !== -1 ||
        (item.score1 && item.score1.toString().indexOf(filter) !== -1) ||
        (item.score2 && item.score2.toString().indexOf(filter) !== -1) ||
        (item.matchQuality && item.matchQuality.toString().indexOf(filter) !== -1);
    };
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
