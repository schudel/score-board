import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatchService} from '../../../services/rest/match.service';
import {Match} from '../../../models/match';
import {NotificationService} from '../../../services/common/notification.service';
import {Player} from '../../../models/player';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Role} from '../../../models/role';
import {MatchHelperService} from '../../../services/common/match-helper.service';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.scss']
})
export class MatchComponent implements OnInit, OnDestroy {
  subscriptions = [];
  match: Match;
  isEditMode = false;
  isCreateMode = false;
  currentPlayer: Player;
  private matchId: string;

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private matchService: MatchService,
              private notificationService: NotificationService,
              private authenticationService: AuthenticationService) {
  }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    const path = this.activeRoute.snapshot.routeConfig.path;
    if (path === 'match/view/:id') {
      this.isEditMode = false;
      this.isCreateMode = false;
      if (routeParams == null || routeParams.id == null) {
        return;
      }
      this.matchId = routeParams.id;
    } else if (path === 'match/edit/:id') {
      this.isEditMode = true;
      this.isCreateMode = false;
      if (routeParams == null || routeParams.id == null) {
        return;
      }
      this.matchId = routeParams.id;
    } else if (path === 'match/create') {
      this.isEditMode = false;
      this.isCreateMode = true;
    } else {
      this.isEditMode = false;
      this.isCreateMode = false;
      return;
    }
    if (!this.isCreateMode) {
      this.subscriptions.push(this.matchService.get(this.matchId, false).subscribe(m => {
        this.match = m;
      }));
    }
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  public get canUpdate(): boolean {
    return MatchHelperService.allowedToEdit(this.match, this.currentPlayer);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
