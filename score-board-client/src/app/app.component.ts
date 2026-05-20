import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {AuthenticationService} from './services/rest/authentication.service';
import {Router} from '@angular/router';
import {Player} from './models/player';
import {Role} from './models/role';
import {DOCUMENT} from '@angular/common';
import {PageScrollService} from 'ngx-page-scroll-core';
import {ThemeService} from './services/common/theme.service';
import {LiveMatchService} from './services/rest/live-match.service';

declare const initConstellation: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  subscriptions = [];
  title = 'Scoreboard';
  currentPlayer: Player;
  isDarkTheme: boolean;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private pageScrollService: PageScrollService,
    @Inject(DOCUMENT) private document: any,
    private themeService: ThemeService,
    private liveMatchService: LiveMatchService
  ) { }

  ngOnInit(): void {
    initConstellation();
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => {
      this.currentPlayer = x;
      if (x) {
        this.liveMatchService.loadLiveMatches();
      }
    }));
    this.subscriptions.push(this.themeService.isDarkTheme.subscribe(x => this.isDarkTheme  = x));
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  logout() {
    this.authenticationService.logout();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
