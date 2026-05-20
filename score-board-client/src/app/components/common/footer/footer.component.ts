import {Component, OnDestroy, OnInit} from '@angular/core';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Player} from '../../../models/player';
import {ThemeService} from '../../../services/common/theme.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit, OnDestroy {
  subscriptions = [];
  currentPlayer: Player;
  isDarkTheme: boolean;
  currentYear = (new Date()).getFullYear();

  constructor(private authenticationService: AuthenticationService,
              private themeService: ThemeService) { }

  ngOnInit() {
    const subscription = this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x);
    const subscription2 = this.themeService.isDarkTheme.subscribe(x => this.isDarkTheme  = x);
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
    this.subscriptions.push(subscription2);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
