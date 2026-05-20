import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {Role} from '../../../models/role';
import {Player} from '../../../models/player';
import {AuthenticationService} from '../../../services/rest/authentication.service';

@Component({
  selector: 'app-side-menu',
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss']
})
export class SideMenuComponent implements OnInit, OnDestroy {

  constructor(private authenticationService: AuthenticationService) { }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }
  subscriptions = [];
  currentPlayer: Player;

  @Output()
  menuButtonClicked = new EventEmitter();

  ngOnInit() {
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => {
      this.currentPlayer = x;
    }));
  }

  onMenuButtonClicked() {
    this.menuButtonClicked.emit();
  }

  logout() {
    this.authenticationService.logout();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
