import {EventEmitter, Injectable, OnDestroy} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HubConnection, HubConnectionBuilder} from '@aspnet/signalr';
import {LiveMatch} from '../../models/live-match';
import {BaseRestService} from './base-rest.service';
import {HttpClient} from '@angular/common/http';
import {NotificationService} from '../common/notification.service';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {Invitation} from '../../models/invitation';
import {LiveMatchHelperService} from '../common/live-match-helper.service';

@Injectable({
  providedIn: 'root'
})
export class LiveMatchService extends BaseRestService implements OnDestroy {
  private scoreBoarServerUrlWebSocket = environment.scoreBoarServerUrl + '/live'; // URL to websocket
  private scoreBoarServerUrlWebApi = environment.scoreBoarServerUrl + '/api/liveMatch'; // URL to web api
  private connectionIsEstablished = false;
  private hubConnection: HubConnection;
  private subscriptions = [];
  liveMatches: LiveMatch[];
  localStorageKey = 'liveMatches';

  matchUpdateReceived = new EventEmitter<LiveMatch[]>();
  connectionEstablished = new EventEmitter<boolean>();
  localLiveMatchesChanged = new EventEmitter();

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  IsConnectionEstablished(): boolean {
    return this.connectionIsEstablished;
  }

  sendMatchUpdate(liveMatch: LiveMatch) {
    this.hubConnection.invoke('UpdateMatch', liveMatch)
      .then()
      .catch(err => console.error(err.toString()));
  }

  getAll(): Observable<any> {
    return this.http.get<LiveMatch>(this.scoreBoarServerUrlWebApi)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get live matches', [])));
  }

  getAllDistinct(): Observable<any> {
    return this.http.get<LiveMatch>(this.scoreBoarServerUrlWebApi + '/distinct')
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get live matches distinct', [])));
  }

  get(id: string): Observable<any> {
    return this.http.get<LiveMatch>(this.scoreBoarServerUrlWebApi + '/' + id )
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get live match for ' + id, [])));
  }

  getByMatch(matchId: string): Observable<any> {
    return this.http.get<LiveMatch>(this.scoreBoarServerUrlWebApi + '/match?matchId=' + matchId)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get live matches for match id ' + matchId, [])));
  }

  deleteAll() {
    return this.http.delete<LiveMatch>(this.scoreBoarServerUrlWebApi)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('delete all live matches', [])));
  }

  invitePlayer(invitation: Invitation) {
    return this.http.post<Invitation>(this.scoreBoarServerUrlWebApi + '/invite', invitation)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('players invited')));
  }

  private createConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.scoreBoarServerUrlWebSocket)
      .build();
  }

  private startConnection(): void {
    this.hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Live-Match Hub connection started');
        this.connectionEstablished.emit(true);
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
        setTimeout(this.startConnection, 1000);
      });
  }

  loadLiveMatches(): void {
    this.liveMatches = JSON.parse(localStorage.getItem(this.localStorageKey));
    if (!this.liveMatches) {
      this.liveMatches = [];
    }
    if (!this.connectionIsEstablished) {
      this.createConnection();
      this.registerOnServerEvents();
      this.startConnection();
    }
    this.subscriptions.push(this.getAllDistinct().subscribe(x => {
      this.liveMatches = [];
      if (!x) {
        return;
      }
      for (const liveMatch of x) {
        this.liveMatches = LiveMatchHelperService.processLiveMatch(this.liveMatches, liveMatch);
      }
      localStorage.setItem(this.localStorageKey, JSON.stringify(this.liveMatches));
      this.localLiveMatchesChanged.emit();
    }));
  }

  private registerOnServerEvents(): void {
    this.hubConnection.on('ReceiveMatchUpdate', (data: any) => {
      this.liveMatches = LiveMatchHelperService.processLiveMatch(this.liveMatches, data);
      localStorage.setItem(this.localStorageKey, JSON.stringify(this.liveMatches));
      this.matchUpdateReceived.emit(this.liveMatches);
      this.localLiveMatchesChanged.emit();
    });
  }

  getLocalLiveMatch(matchId: string): LiveMatch {
    for (const lm of this.liveMatches) {
      if (matchId.toLowerCase() === lm.matchId.toLowerCase()) {
        return lm;
      }
    }
  }

  clearLocalStorage(): void {
    // remove player from local storage to log player out
    localStorage.removeItem(this.localStorageKey);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
