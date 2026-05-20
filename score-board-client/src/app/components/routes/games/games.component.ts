import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {GameService} from '../../../services/rest/game.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {Game} from '../../../models/game';
import {Role} from '../../../models/role';
import {Player} from '../../../models/player';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {ModalDialogComponent} from '../../common/modal-dialog/modal-dialog.component';

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.scss']
})
export class GamesComponent implements OnInit, OnDestroy {
  subscriptions = [];
  games: Game[];
  dataSource = new MatTableDataSource();
  currentPlayer: Player;
  displayedColumns: string[] = ['Image', 'Name', 'Type', 'Genre', 'Details', 'Edit', 'Remove'];

  @ViewChild(MatSort, {static: true}) sort: MatSort;

  constructor(private gameService: GameService,
              private authenticationService: AuthenticationService,
              public dialog: MatDialog) { }

  ngOnInit() {
    this.subscriptions.push(this.gameService.getAll().subscribe(
      g => {
        this.games = g;
        this.dataSource = new MatTableDataSource<Game>(this.games);
        this.dataSource.sort = this.sort;
        this.dataSource.filterPredicate = this.createFilter();
      }
    ));
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  removeGame(game: Game) {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@removeGameTitle:Remove Game`,
        message: $localize`:@@removeGameTextPart1:Are you sure you want to remove the Game` + ' \"' + game.name + '\" ' + $localize`:@@removeGameTextPart2:with all data` + '?'}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        console.log('TODO: remove Game');
      }
    }));
  }

  createFilter(): (item: Game, filter: string) => boolean {
    return (item, filter) => {
      return item.name.toLowerCase().indexOf(filter) !== -1 ||
        item.genre.toLowerCase().indexOf(filter) !== -1 || item.type.toLowerCase().indexOf(filter) !== -1;
    };
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
