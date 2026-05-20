import {Component, Input, NgZone, OnDestroy, OnInit} from '@angular/core';
import {LiveMatchService} from '../../../services/rest/live-match.service';
import {LiveMatch} from '../../../models/live-match';
import {Match} from '../../../models/match';
import {Team} from '../../../models/team';
import {ActivatedRoute} from '@angular/router';
import {Constants} from '../../../constants/constants';

class LiveMatchExtension {
  game: string;
  liveDurationString: string;
  team1: Team;
  team2: Team;
}

@Component({
  selector: 'app-live',
  templateUrl: './live.component.html',
  styleUrls: ['./live.component.scss']
})
export class LiveComponent implements OnInit, OnDestroy {
  subscriptions = [];
  liveMatches: LiveMatch[];
  canSendMessage: boolean;
  matches: Match[];
  liveMapExtension: Map<string, LiveMatchExtension> = new Map<string, LiveMatchExtension>();
  accordionDisabledInternal = null;
  expandedMatchId: string;

  constructor(private liveMatchService: LiveMatchService,
              private ngZone: NgZone,
              private activeRoute: ActivatedRoute) {
    this.liveMatches = [];
  }

  @Input()
  set accordionDisabled(val: boolean) {
    this.accordionDisabledInternal = val;
  }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    if (routeParams != null && routeParams.id != null) {
      this.expandedMatchId = routeParams.id;
    }
    this.liveMatches = this.liveMatchService.liveMatches;
    this.subscriptions.push(this.liveMatchService.localLiveMatchesChanged.subscribe(() => {
      this.liveMatches = this.liveMatchService.liveMatches;
    }));
    this.subscriptions.push(this.liveMatchService.connectionEstablished.subscribe(() => {
      this.canSendMessage = true;
    }));
    this.subscriptions.push(this.liveMatchService.matchUpdateReceived.subscribe((liveMatches: LiveMatch[]) => {
      this.ngZone.run(() => {
        this.liveMatches = liveMatches;
      });
    }));
    if (this.accordionDisabledInternal === null) {
      this.accordionDisabledInternal = false;
    }
  }

  onLiveDurationChanged(durationString: string, liveMatch: LiveMatch) {
    if (!durationString || !liveMatch) {
      return;
    }
    let extension = new LiveMatchExtension();
    if (this.liveMapExtension.has(liveMatch.id)) {
      extension = this.liveMapExtension.get(liveMatch.id);
    }
    extension.liveDurationString = durationString;
    this.liveMapExtension.set(liveMatch.id, extension);
  }

  onTeam1Changed(team: Team, liveMatch: LiveMatch) {
    if (!team || !liveMatch) {
      return;
    }
    let extension = new LiveMatchExtension();
    if (this.liveMapExtension.has(liveMatch.id)) {
      extension = this.liveMapExtension.get(liveMatch.id);
    }
    extension.team1 = team;
    this.liveMapExtension.set(liveMatch.id, extension);
  }

  onTeam2Changed(team: Team, liveMatch: LiveMatch) {
    if (!team || !liveMatch) {
      return;
    }
    let extension = new LiveMatchExtension();
    if (this.liveMapExtension.has(liveMatch.id)) {
      extension = this.liveMapExtension.get(liveMatch.id);
    }
    extension.team2 = team;
    this.liveMapExtension.set(liveMatch.id, extension);
  }

  onGameChanged(game: string, liveMatch: LiveMatch) {
    if (!game || !liveMatch) {
      return;
    }
    let extension = new LiveMatchExtension();
    if (this.liveMapExtension.has(liveMatch.id)) {
      extension = this.liveMapExtension.get(liveMatch.id);
    }
    extension.game = game;
    this.liveMapExtension.set(liveMatch.id, extension);
  }

  getTranslatedState(matchStateCode: string): string {
    return matchStateCode;
    // TODO: fix localization
    // return $localize`:@@${matchStateCode}:${matchStateCode}`;
  }

  getDateFormat(): string {
    return Constants.DateFormat;
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
