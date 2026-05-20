import {Component, Input, OnInit, ViewChild} from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {Rating} from '../../../models/rating';
import {Game} from '../../../models/game';
import {RatingHelperService} from '../../../services/common/rating-helper.service';
import {GameHelperService} from '../../../services/common/game-helper.service';

@Component({
  selector: 'app-ranking-widget',
  templateUrl: './ranking-widget.component.html',
  styleUrls: ['./ranking-widget.component.scss']
})
export class RankingWidgetComponent implements OnInit {
  ratingsInternal: Rating[];
  dataSource = new MatTableDataSource<Rating>();
  displayedColumns: string[] = ['Ranking', 'PlayerName', 'ConservativeRating', 'Mean', 'StandardDeviation'];
  games: Game[];
  gameId: string;

  @Input()
  set ratings(val: Rating[]) {
    this.ratingsInternal = val;
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
      this.initDataSource(this.gameId);
    }
  }

  constructor() {
    this.ratingsInternal = [];
    this.games = [];
  }

  @Input()
  set game(val: Game) {
    this.gameId = val.id;
    this.initDataSource(this.gameId);
  }

  @ViewChild(MatSort, {static: true}) sort: MatSort;

  ngOnInit() {
    this.initDataSource(this.gameId);
  }

  initDataSource(gameId) {
    this.dataSource = new MatTableDataSource<Rating>(RatingHelperService.filterByGame(gameId, this.ratingsInternal));
    this.dataSource.sortingDataAccessor = (item: Rating, property) => {
      switch (property) {
        case 'Ranking':
          return item.conservativeRating;
        case 'PlayerName':
          return item.player.playerName;
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

  getRanking(element: Rating): number {
    return RatingHelperService.getRanking(element.player, element.game, this.ratingsInternal);
  }

  changeGame($event: any) {
    this.initDataSource($event);
  }

  createFilter(): (item: Rating, filter: string) => boolean {
    return (item, filter) => {
      return item.player.playerName.toLowerCase().indexOf(filter) !== -1 ||
        (item.conservativeRating && item.conservativeRating.toString().indexOf(filter) !== -1) ||
        (item.standardDeviation && item.standardDeviation.toString().indexOf(filter) !== -1) ||
        (item.mean && item.mean.toString().indexOf(filter) !== -1);
    };
  }
}
