import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {Match} from '../../../models/match';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Player} from '../../../models/player';
import {Game} from '../../../models/game';
import {MatchState} from '../../../models/match-state';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {MatchService} from '../../../services/rest/match.service';
import {NotificationService} from '../../../services/common/notification.service';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {GameService} from '../../../services/rest/game.service';
import {PlayerService} from '../../../services/rest/player.service';
import {TeamService} from '../../../services/rest/team.service';
import {LiveMatchService} from '../../../services/rest/live-match.service';
import {Guid} from '../../../helpers/guid';
import {Team} from '../../../models/team';
import {DateHelper} from '../../../helpers/date-helper';
import {LiveMatch} from '../../../models/live-match';
import {Role} from '../../../models/role';
import {map, startWith} from 'rxjs/operators';
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem} from '@angular/cdk/drag-drop';
import {Invitation} from '../../../models/invitation';
import {MatchHelperService} from '../../../services/common/match-helper.service';

@Component({
  selector: 'app-match-detail',
  templateUrl: './match-detail.component.html',
  styleUrls: ['./match-detail.component.scss']
})
export class MatchDetailComponent implements OnInit, OnDestroy {

  @Input()
  set match(val: Match) {
    if (!val) {
      return;
    }
    this.matchInternal = val;
    if (!this.isCreateMode && !this.canUpdate && this.matchInternal && !this.isAdmin) {
      this.router.navigate(['/match/view/' + this.matchInternal.id]).then(() => {});
    }
    this.team1Changed.emit(this.matchInternal.team1);
    this.team2Changed.emit(this.matchInternal.team2);
    this.gameChanged.emit(this.matchInternal.game.name);
    this.initForm();
  }
  @Input()
  set liveMatch(val: LiveMatch) {
    if (!val) {
      return;
    }
    this.liveMatchInternal = val;
    if (!this.matchInternal || this.matchInternal.id !== this.liveMatchInternal.matchId) {
      this.subscriptions.push(this.matchService.get(this.liveMatchInternal.matchId, false).subscribe(x => {
        if (!x) {
          return;
        }
        x.score1 = this.liveMatchInternal.score1;
        x.score2 = this.liveMatchInternal.score2;
        x.state = this.liveMatchInternal.state;
        this.match = x;
        this.startTimer();
      }));
    }
    this.subscriptions.push(this.liveMatchService.localLiveMatchesChanged.subscribe(() => {
      if (!this.matchInternal) {
        return;
      }
      const lm = this.liveMatchService.getLocalLiveMatch(this.matchInternal.id);
      if (!lm) {
        return;
      }
      this.matchInternal.state = lm.state;
      this.matchForm.patchValue({
        state: MatchState.getMatchState(this.matchInternal.state)
      });
      this.matchInternal.score1 = lm.score1;
      this.matchForm.patchValue({
        score1: this.matchInternal.score1
      });
      this.matchInternal.score2 = lm.score2;
      this.matchForm.patchValue({
        score2: this.matchInternal.score2
      });
    }));
  }

  constructor(private router: Router,
              private matchService: MatchService,
              private notificationService: NotificationService,
              private authenticationService: AuthenticationService,
              private gameService: GameService,
              private playerService: PlayerService,
              private teamService: TeamService,
              private liveMatchService: LiveMatchService) {
    this.matchForm = new FormGroup({
      game: new FormControl('', Validators.required),
      team1Name: new FormControl('', Validators.required),
      team2Name: new FormControl('', Validators.required),
      score1: new FormControl('', Validators.required),
      score2: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      startTime: new FormControl('', Validators.required),
      stopTime: new FormControl(''),
      duration: new FormControl(''),
      matchQuality: new FormControl(''),
      sendInvitation: new FormControl('')
    });
    this.allGames = [];
    this.allPlayers = [];
    this.allMatchStates = MatchState.getMatchStates();
    this.teamNames = [];
    this.team1Players = [];
    this.team2Players = [];
  }

  // convenience getter for easy access to form fields
  get form() { return this.matchForm.controls; }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  public get canUpdate(): boolean {
    return MatchHelperService.allowedToEdit(this.matchInternal, this.currentPlayer);
  }
  subscriptions = [];
  submitted = false;
  matchForm: FormGroup;
  error = '';
  currentPlayer: Player;
  allGames: Game[];
  allPlayers: Player[];
  team1Players: Player[];
  team2Players: Player[];
  allMatchStates: MatchState[];
  teamNames: string[];
  filteredTeam1Names: Observable<string[]>;
  filteredTeam2Names: Observable<string[]>;
  canSendMessage: boolean;
  matchInternal: Match;
  liveMatchInternal: LiveMatch;
  interval;
  liveDuration;
  disableButtons;

  @Input() isCreateMode: boolean;
  @Input() isEditMode: boolean;
  @Input() isLiveMode: boolean;

  @Output() team1Changed = new EventEmitter<Team>();
  @Output() team2Changed = new EventEmitter<Team>();
  @Output() gameChanged = new EventEmitter<string>();
  @Output() liveDurationChanged = new EventEmitter<string>();

  private static transformSeconds(seconds): string {
    const times = {
      year: 31557600,
      month: 2629746,
      day: 86400,
      hour: 3600,
      minute: 60,
      second: 1
    };
    let timeString = '';
    for (const key in times) {
      if (Math.floor(seconds / times[key]) > 0) {
        let s = Math.floor(seconds / times[key]).toString();
        if (s.length === 1) {
          s = '0' + s;
        }
        timeString += s + ':';
        seconds = seconds - times[key] * Math.floor(seconds / times[key]);
      }
    }
    return timeString.substr(0, timeString.length - 1);
  }

  ngOnInit() {
    this.initFilters();
    if (this.isCreateMode) {
      // create new Match
      this.matchInternal = new Match();
      this.matchInternal.id = Guid.newGuid().ToString();
      this.matchInternal.team1 = new Team();
      this.matchInternal.team1.id = Guid.newGuid().ToString();
      this.matchInternal.team1.player1 = new Player();
      this.matchInternal.team2 = new Team();
      this.matchInternal.team2.id = Guid.newGuid().ToString();
      this.matchInternal.team2.player1 = new Player();
      this.matchInternal.game = new Game();
      this.matchInternal.score1 = 0;
      this.matchInternal.score2 = 0;
      this.matchInternal.state = MatchState.Playing.matchStateCode;
      this.matchInternal.startTime = DateHelper.GetDate(new Date().toString());
    }
    this.initForm();

    this.subscriptions.push(this.playerService.getAll().subscribe(x => {
      this.allPlayers = x;
    }));
    this.subscriptions.push(this.gameService.getAll().subscribe(x => {
      this.allGames = x;
    }));
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => {
      this.currentPlayer = x;
      if (!this.isCreateMode && !this.canUpdate && this.matchInternal && !this.isAdmin) {
        this.router.navigate(['/match/view/' + this.matchInternal.id]).then(() => {});
      }
    }));
    this.canSendMessage = this.liveMatchService.IsConnectionEstablished();
    this.subscriptions.push(this.liveMatchService.connectionEstablished.subscribe(established => {
      this.canSendMessage = established;
    }));
  }

  initForm() {
    if (!this.isCreateMode) {
      if (!this.matchInternal) {
        return;
      }
      this.matchForm.setValue({
        game: this.matchInternal.game.id,
        team1Name: this.matchInternal.team1.name,
        team2Name: this.matchInternal.team2.name,
        score1: this.matchInternal.score1,
        score2: this.matchInternal.score2,
        state: MatchState.getMatchState(this.matchInternal.state),
        startTime: DateHelper.GetDate(this.matchInternal.startTime.toString()),
        stopTime: ' ',
        duration: ' ',
        matchQuality: this.matchInternal.matchQuality,
        sendInvitation: false
      });
      if (this.matchInternal.team1.player1) {
        this.team1Players.push(this.matchInternal.team1.player1);
        this.removePlayerFromAllPlayers(this.matchInternal.team1.player1.id);
      }
      if (this.matchInternal.team1.player2) {
        this.team1Players.push(this.matchInternal.team1.player2);
        this.removePlayerFromAllPlayers(this.matchInternal.team1.player2.id);
      }
      if (this.matchInternal.team2.player1) {
        this.team2Players.push(this.matchInternal.team2.player1);
        this.removePlayerFromAllPlayers(this.matchInternal.team2.player1.id);
      }
      if (this.matchInternal.team2.player2) {
        this.team2Players.push(this.matchInternal.team2.player2);
        this.removePlayerFromAllPlayers(this.matchInternal.team2.player2.id);
      }
      if (this.matchInternal.stopTime) {
        this.matchForm.patchValue({
          stopTime: DateHelper.GetDate(this.matchInternal.stopTime.toString())
        });
      }
      if (this.matchInternal.duration) {
        this.matchForm.patchValue({
          duration: this.matchInternal.duration
        });
      }
    } else if (this.isCreateMode) {
      this.matchForm.patchValue({
        score1: this.matchInternal.score1,
        score2: this.matchInternal.score2,
        state: MatchState.getMatchState(this.matchInternal.state),
        startTime: new Date(),
      });
    }
    if (!this.isEditMode && !this.isCreateMode) {
      this.form.game.disable();
      this.form.team1Name.disable();
      this.form.team2Name.disable();
      this.form.score1.disable();
      this.form.score2.disable();
      this.form.state.disable();
      this.form.startTime.disable();
      this.form.stopTime.disable();
    }
    if (!this.isAdmin && this.isEditMode && this.matchInternal.state === MatchState.Done.matchStateCode) {
      this.form.state.disable();
    }
    this.form.duration.disable();
    this.form.matchQuality.disable();
    if (this.isEditMode &&
      (this.matchInternal.state === MatchState.Done.matchStateCode ||
        this.matchInternal.state === MatchState.Aborted.matchStateCode)) {
      this.form.sendInvitation.disable();
    }
  }

  onSubmit() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.matchForm.invalid) {
      return;
    }
    if (!this.team1Players || this.team1Players.length === 0 || !this.team2Players || this.team2Players.length === 0) {
      this.notificationService.warning($localize`:@@onePlayerInEachTeam:At least one Player has to be in each Team.`, $localize`:@@invalidTitle:Invalid`);
      return;
    }
    // take values from form
    this.matchInternal.game.id = this.form.game.value;
    this.matchInternal.state = this.form.state.value.matchStateCode;
    this.matchInternal.team1.player1.id = this.team1Players[0].id;
    if (this.team1Players.length === 2) {
      this.matchInternal.team1.player2 = new Player();
      this.matchInternal.team1.player2.id = this.team1Players[1].id;
    }
    this.matchInternal.team2.player1.id = this.team2Players[0].id;
    if (this.team2Players.length === 2) {
      this.matchInternal.team2.player2 = new Player();
      this.matchInternal.team2.player2.id = this.team2Players[1].id;
    }
    this.matchInternal.startTime = this.form.startTime.value;
    this.matchInternal.stopTime = this.form.stopTime.value;
    this.matchInternal.score1 = this.form.score1.value;
    this.matchInternal.score2 = this.form.score2.value;
    this.matchInternal.team1.name = this.form.team1Name.value;
    this.matchInternal.team2.name = this.form.team2Name.value;

    if (this.isCreateMode) {
      this.subscriptions.push(this.matchService.add(this.matchInternal).subscribe(() => {
        if (this.matchInternal.state === MatchState.Scheduled.matchStateCode ||
          this.matchInternal.state === MatchState.Playing.matchStateCode) {
          this.updateMatch(this.matchInternal.startTime);
        }
        this.notificationService.success($localize`:@@matchSuccessfullyAddedText:New Match was added!`, $localize`:@@matchSuccessfullyAddedTitle:Add Match`);
        // navigate to live matches
        if (this.matchInternal.state === MatchState.Scheduled.matchStateCode ||
          this.matchInternal.state === MatchState.Playing.matchStateCode) {
          this.router.navigate(['/live/' + this.matchInternal.id]).then(() => {});
        } else {
          this.router.navigate(['/matches']).then(() => {});
        }
      }));
      if (this.matchInternal.state === MatchState.Scheduled.matchStateCode ||
        this.matchInternal.state === MatchState.Playing.matchStateCode) {
        if (this.form.sendInvitation.value) {
          this.invitePlayers();
        }
      }
    } else if (this.isEditMode) {
      this.subscriptions.push(this.matchService.update(this.matchInternal).subscribe(() => {
        this.notificationService.success($localize`:@@matchSuccessfullyUpdatedText:The Match was updated!`,
          $localize`:@@matchSuccessfullyUpdatedTitle:Update Match`);
      }));
    }
  }

  private filterTeamNames(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.teamNames.filter(option => option.toLowerCase().includes(filterValue));
  }

  private initFilters() {
    this.subscriptions.push(this.teamService.getNames().subscribe(x => {
      this.teamNames = x;
      this.filteredTeam1Names = this.form.team1Name.valueChanges
        .pipe(
          startWith(''),
          map(value => {
            return this.filterTeamNames(value);
          })
        );
      this.filteredTeam2Names = this.form.team2Name.valueChanges
        .pipe(
          startWith(''),
          map(value => {
            return this.filterTeamNames(value);
          })
        );
    }));
  }

  private removePlayerFromAllPlayers(id: string) {
    let index;
    for (let i = 0; i < this.allPlayers.length; i++) {
      if (this.allPlayers[i].id === id) {
        index = i;
        break;
      }
    }
    if (index > -1) {
      this.allPlayers.splice(index, 1);
    }
  }

  drop(event: CdkDragDrop<Player[], any>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data,
        event.previousIndex,
        event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex);
    }
  }

  maxPredicate(drag: CdkDrag, drop: CdkDropList) {
    if (drop.id === 'team1-list' || drop.id === 'team2-list') {
      if (!drop.data) {
        return true;
      }
      return drop.data.length < 2;
    }
    return true;
  }

  startTimer(): void {
    if (!this.isLiveMode) {
      return;
    }
    if (this.matchInternal.state === MatchState.Playing.matchStateCode) {
      this.interval = setInterval(() => {
        this.liveDuration = MatchDetailComponent.transformSeconds((
          new Date().valueOf() - DateHelper.GetDate(this.matchInternal.startTime.toString()).valueOf()
        ) / 1000);
        this.liveDurationChanged.emit(this.liveDuration);
      }, 1000);
    }
    /*
    else if (this.matchInternal.State === MatchState.Scheduled.displayName) {
      this.liveDurationChanged.emit(this.matchInternal.StartTime.toString());
      this.interval = setInterval(() => {
        if (new Date().valueOf() > DateHelper.GetDate(this.matchInternal.StartTime.toString()).valueOf()) {
          this.startMatch();
        }
      }, 1000);
    }
    */
  }

  private pauseTimer() {
    clearInterval(this.interval);
  }

  finishLiveGame() {
    this.disableButtons = true;
    this.matchInternal.state = MatchState.Done.matchStateCode;
    this.matchInternal.stopTime = new Date();
    this.subscriptions.push(this.matchService.update(this.matchInternal).subscribe(() => {
      this.updateMatch(this.matchInternal.stopTime);
      this.notificationService.success($localize`:@@matchSuccessfullyFinishedText:The Match was finished!`, $localize`:@@matchSuccessfullyFinishedTitle:Finish Match`);
    }));
  }

  countDown(team: string) {
    this.disableButtons = true;
    if (team === 'team1') {
      if (this.matchInternal.score1 === 0) {
        this.disableButtons = false;
        return;
      }
      this.matchInternal.score1--;
      this.matchForm.patchValue({
        score1: this.matchInternal.score1
      });
    } else if (team === 'team2') {
      if (this.matchInternal.score2 === 0) {
        this.disableButtons = false;
        return;
      }
      this.matchInternal.score2--;
      this.matchForm.patchValue({
        score2: this.matchInternal.score2
      });
    }
    this.updateMatch();
  }

  countUp(team: string) {
    this.disableButtons = true;
    if (team === 'team1') {
      this.matchInternal.score1++;
      this.matchForm.patchValue({
        score1: this.matchInternal.score1
      });
    } else if (team === 'team2') {
      this.matchInternal.score2++;
      this.matchForm.patchValue({
        score2: this.matchInternal.score2
      });
    }
    this.updateMatch();
  }

  startMatch(): void {
    if (!this.canUpdate && !this.isAdmin) {
      return;
    }
    this.disableButtons = true;
    this.pauseTimer();
    // change state to playing
    this.matchInternal.state = MatchState.Playing.matchStateCode;
    this.matchForm.patchValue({
      state: MatchState.Playing
    });
    this.matchInternal.startTime = new Date();
    this.matchForm.patchValue({
      startTime: this.matchInternal.startTime
    });
    this.updateMatch();
    this.startTimer();
  }

  getTranslatedState(matchStateCode: string): string {
    return matchStateCode;
    // TODO: fix localization
    // return $localize`:@@${matchStateCode}:${matchStateCode}`;
  }

  private updateMatch(timeStamp: Date = new Date()) {
    if (this.canSendMessage) {
      const liveMatch = new LiveMatch();
      liveMatch.id = Guid.newGuid().ToString();
      liveMatch.matchId = this.matchInternal.id;
      liveMatch.score1 = this.matchInternal.score1;
      liveMatch.score2 = this.matchInternal.score2;
      liveMatch.state = this.matchInternal.state;
      liveMatch.timeStamp = timeStamp;
      this.liveMatchService.sendMatchUpdate(liveMatch);
    }
    setTimeout( () => { this.disableButtons = false; }, 200 );
  }

  private invitePlayers() {
    // send notifications if enabled
    const invitation = new Invitation();
    invitation.senderId = this.currentPlayer.id;
    invitation.matchId = this.matchInternal.id;
    const receiver = [];
    if (this.matchInternal.team1.player1.id !== invitation.senderId) {
      receiver.push(this.matchInternal.team1.player1.id);
    }
    if (this.matchInternal.team1.player2 && this.matchInternal.team1.player2.id !== invitation.senderId) {
      receiver.push(this.matchInternal.team1.player2.id);
    }
    if (this.matchInternal.team2.player1.id !== invitation.senderId) {
      receiver.push(this.matchInternal.team2.player1.id);
    }
    if (this.matchInternal.team2.player2 && this.matchInternal.team2.player2.id !== invitation.senderId) {
      receiver.push(this.matchInternal.team2.player2.id);
    }
    invitation.receiverIdList = receiver;
    this.subscriptions.push(this.liveMatchService.invitePlayer(invitation).subscribe(() => { }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
