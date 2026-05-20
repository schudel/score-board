import {Component, EventEmitter, NgZone, OnDestroy, OnInit, Output} from '@angular/core';
import {Player} from '../../../models/player';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Role} from '../../../models/role';
import {ThemeService} from '../../../services/common/theme.service';
import {NotificationService} from '../../../services/common/notification.service';
import {LiveMatchService} from '../../../services/rest/live-match.service';
import {LiveMatch} from '../../../models/live-match';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent implements OnInit, OnDestroy {
  subscriptions = [];
  currentPlayer: Player;
  isDarkTheme: boolean;
  notificationsCount: number;
  liveMatchCount: number;

  @Output()
  menuButtonClicked = new EventEmitter();

  @Output()
  panelButtonClicked = new EventEmitter();

  constructor(private authenticationService: AuthenticationService,
              private themeService: ThemeService,
              private notificationService: NotificationService,
              private liveMatchService: LiveMatchService,
              private ngZone: NgZone) {
  }

  ngOnInit() {
    this.liveMatchCount = this.liveMatchService.liveMatches.length;
    this.subscriptions.push(this.liveMatchService.localLiveMatchesChanged.subscribe(() => {
      this.liveMatchCount = this.liveMatchService.liveMatches.length;
    }));
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
    this.subscriptions.push(this.themeService.isDarkTheme.subscribe(x => this.isDarkTheme  = x));
    this.subscriptions.push(this.notificationService.notifications.subscribe(x => {
      if (x) {
        this.notificationsCount  = x.length;
      }
    }));
    this.subscriptions.push(this.liveMatchService.matchUpdateReceived.subscribe((liveMatches: LiveMatch[]) => {
      this.ngZone.run(() => {
        this.liveMatchCount = liveMatches.length;
      });
    }));
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  logout() {
    this.authenticationService.logout();
  }

  onMenuButtonClicked() {
    this.menuButtonClicked.emit();
  }

  onPanelButtonClicked() {
    this.panelButtonClicked.emit();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
