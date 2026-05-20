import {Component, OnDestroy, OnInit} from '@angular/core';
import {CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType} from 'angular-gridster2';
import {NotificationService} from '../../../services/common/notification.service';
import {DashboardWidgetEnum} from '../../../models/dashboard-widget';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Player} from '../../../models/player';
import {PlayerService} from '../../../services/rest/player.service';
import {MatchService} from '../../../services/rest/match.service';
import {Match} from '../../../models/match';
import {first} from 'rxjs/operators';
import {RatingService} from '../../../services/rest/rating.service';
import {Rating} from '../../../models/rating';
import {Router} from '@angular/router';
import {RatingHelperService} from '../../../services/common/rating-helper.service';
import {MatchHelperService} from '../../../services/common/match-helper.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {

  constructor(private router: Router,
              private notificationService: NotificationService,
              private authenticationService: AuthenticationService,
              private playerService: PlayerService,
              private matchService: MatchService,
              private ratingService: RatingService) { }
  subscriptions = [];
  currentPlayer: Player;
  players: Player[];
  matches: Match[];
  recentMatches: Match[];
  epicMatches: Match[];
  myMatches: Match[];
  ratings: Rating[];
  ranking: number;
  highestScore: number;
  options: GridsterConfig;
  dashboard: Array<GridsterItem>;
  editable = false;
  removeItemEnabled = false;
  tempDashboard;

  private static getTime(date?: Date) {
    return date != null ? new Date(date).getTime() : 0;
  }

  ngOnInit() {
    this.options = {
      gridType: GridType.Fit,
      compactType: CompactType.None,
      margin: 20,
      outerMargin: false,
      outerMarginTop: null,
      outerMarginRight: null,
      outerMarginBottom: null,
      outerMarginLeft: null,
      useTransformPositioning: true,
      mobileBreakpoint: 640,
      minCols: 1,
      maxCols: 100,
      minRows: 1,
      maxRows: 100,
      maxItemCols: 100,
      minItemCols: 1,
      maxItemRows: 100,
      minItemRows: 1,
      maxItemArea: 2500,
      minItemArea: 1,
      defaultItemCols: 1,
      defaultItemRows: 1,
      fixedColWidth: 105,
      fixedRowHeight: 105,
      keepFixedHeightInMobile: false,
      keepFixedWidthInMobile: false,
      scrollSensitivity: 10,
      scrollSpeed: 20,
      enableEmptyCellClick: false,
      enableEmptyCellContextMenu: false,
      enableEmptyCellDrop: false,
      enableEmptyCellDrag: false,
      emptyCellDragMaxCols: 50,
      emptyCellDragMaxRows: 50,
      ignoreMarginInRow: false,
      draggable: {
        enabled: false,
        ignoreContentClass: 'no-drag'
      },
      resizable: {
        enabled: false,
      },
      swap: false,
      pushItems: false,
      disablePushOnDrag: false,
      disablePushOnResize: false,
      pushDirections: {north: true, east: true, south: true, west: true},
      pushResizeItems: false,
      displayGrid: DisplayGrid.OnDragAndResize,
      disableWindowResize: false,
      disableWarnings: false,
      scrollToNewItems: false
    };

    // Default Layout
    this.setDefaultLayout();

    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => {
      this.currentPlayer = x;
      if (!this.currentPlayer) {
        return;
      }
      if (this.currentPlayer.settings.dashboardLayout) {
        this.dashboard = JSON.parse(this.currentPlayer.settings.dashboardLayout);
      }
      this.myMatches = this.getMyMatches(this.currentPlayer);
    }));
    this.subscriptions.push(this.playerService.getAll().subscribe(
      p => {
        this.players = p;
        this.ranking = RatingHelperService.getRanking(this.currentPlayer, null, this.ratings);
      }
    ));
    this.subscriptions.push(this.matchService.getAll().subscribe(m => {
      this.matches = m;
      this.recentMatches = this.getRecentMatches();
      this.epicMatches = this.getEpicMatches();
      if (this.currentPlayer) {
        this.myMatches = this.getMyMatches(this.currentPlayer.id);
      }
    }));
    this.subscriptions.push(this.ratingService.getAll().subscribe(r => {
      this.ratings = r.sort(RatingHelperService.sortByConservativeRating);
      if (this.ratings && this.ratings.length > 0) {
        this.highestScore = this.ratings[0].conservativeRating;
        this.ranking = RatingHelperService.getRanking(this.currentPlayer, null, this.ratings);
      }
    }));
  }

  removeItem(item) {
    if (this.dashboard.length > 1) {
      this.dashboard.splice(this.dashboard.indexOf(item), 1);
    }
  }

  addItem() {
    if (this.dashboard.length < 10) {
      this.dashboard.push({x: 0, y: 0, cols: 1, rows: 1, label: DashboardWidgetEnum.ScoreBoardInfo});
    }
  }

  toggleEditable() {
    if (this.editable) {
      this.tempDashboard = [];
      this.disableEditMode();
      this.updateDashboardLayout();
    } else {
      this.tempDashboard = JSON.parse(JSON.stringify(this.dashboard)); // deep copy dashboard
      this.enableEditMode();
    }
  }

  changedOptions() {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
  }

  enableEditMode() {
    if (this.options !== undefined) {
      this.options.resizable.enabled = true;
      this.options.draggable.enabled = true;
      this.options.outerMargin = true;
      this.options.displayGrid = DisplayGrid.OnDragAndResize;
      this.options.pushItems = true;
      this.changedOptions();
      this.editable = true;
      this.removeItemEnabled = true;
    }
  }

  disableEditMode() {
    if (this.options !== undefined) {
      this.options.resizable.enabled = false;
      this.options.draggable.enabled = false;
      this.options.outerMargin = false;
      this.options.displayGrid = DisplayGrid.None;
      this.options.pushItems = false;
      this.changedOptions();
      this.editable = false;
      this.removeItemEnabled = false;
    }
  }

  getMyMatches(playerId): Match[] {
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

  private getRecentMatches(): Match[] {
    if (!this.matches) {
      return [];
    }
    return this.matches.sort((a: Match, b: Match) => {
      return DashboardComponent.getTime(a.startTime) - DashboardComponent.getTime(b.startTime);
    });
  }

  private getEpicMatches() {
    if (!this.matches) {
      return [];
    }
    return this.matches.sort(MatchHelperService.sortMatchesByQuality);
  }

  private updateDashboardLayout() {
    if (!this.currentPlayer || !this.dashboard) {
      return;
    }
    this.currentPlayer.settings.dashboardLayout = JSON.stringify(this.dashboard);
    this.subscriptions.push(this.playerService.updateSettings(this.currentPlayer.id, this.currentPlayer.settings)
      .pipe(first())
      .subscribe(
        () => {
          this.notificationService.success($localize`:@@updateDashboardText:Your Dashboard Layout has been updated.`,
          $localize`:@@updateDashboardTitle:Update Dashboard`);
          this.authenticationService.setCurrentPlayerValue(this.currentPlayer);
        },
        error => {
          this.notificationService.success(error,
            $localize`:@@updateDashboardFailedTitle:Update Dashboard failed`);
        }));
  }

  getFavoriteGame(): string {
    return MatchHelperService.getFavoriteGame(this.matches, this.currentPlayer);
  }

  getFavoriteTeamMember(): string {
    return MatchHelperService.getFavoriteTeamMember(this.matches, this.currentPlayer);
  }

  setDefaultLayout() {
    this.dashboard = [
      {cols: 3, rows: 4, y: 0, x: 3, label: DashboardWidgetEnum.ScoreBoardInfo},
      {cols: 6, rows: 4, y: 4, x: 0, label: DashboardWidgetEnum.LiveMatches},
      {cols: 2, rows: 2, y: 0, x: 6, label: DashboardWidgetEnum.NoOneWidget},
      {cols: 5, rows: 4, y: 0, x: 8, label: DashboardWidgetEnum.Ranking},
      {cols: 3, rows: 4, y: 0, x: 0, label: DashboardWidgetEnum.MyStatistics},
      {cols: 7, rows: 4, y: 4, x: 6, label: DashboardWidgetEnum.MyMatches},
      {cols: 2, rows: 2, y: 2, x: 6, label: DashboardWidgetEnum.StartMatch}
    ];
  }

  cancel() {
    this.dashboard = this.tempDashboard;
    this.disableEditMode();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
