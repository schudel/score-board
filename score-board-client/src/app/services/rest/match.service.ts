import { Injectable } from '@angular/core';
import {BaseRestService} from './base-rest.service';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {Match} from '../../models/match';
import {NotificationService} from '../common/notification.service';

@Injectable({
  providedIn: 'root'
})
export class MatchService extends BaseRestService {
  private scoreBoarServerUrl = environment.scoreBoarServerUrl + '/api/match'; // URL to web api

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  getAll(slim: boolean = true): Observable<any> {
    return this.http.get<Match>(this.scoreBoarServerUrl + '?slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get matches', [])));
  }

  getByGame(gameId: string, slim: boolean = false): Observable<any> {
    return this.http.get<Match>(this.scoreBoarServerUrl + '/game?gameId=' + gameId + '&slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get matches for game id ' + gameId, [])));
  }

  getByPlayer(playerId: string, slim: boolean = false): Observable<any> {
    return this.http.get<Match>(this.scoreBoarServerUrl + '/player?playerId=' + playerId + '&slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get matches for player id ' + playerId, [])));
  }

  get(id: string, slim: boolean = false): Observable<any> {
    return this.http.get<Match>(this.scoreBoarServerUrl + '/' + id + '?slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get match for ' + id, [])));
  }

  add(match: Match) {
    delete match.team1.player1.role;
    delete match.team1.player1.password;
    if (match.team1.player2) {
      delete match.team1.player2.role;
      delete match.team1.player2.password;
    }
    delete match.team2.player1.role;
    delete match.team2.player1.password;
    if (match.team2.player2) {
      delete match.team2.player2.role;
      delete match.team2.player2.password;
    }
    return this.http.post<Match>(this.scoreBoarServerUrl, match)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('add match ' + match.id)));
  }

  update(match: Match) {
    return this.http.put<Match>(this.scoreBoarServerUrl + '/' + match.id, match)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('update match ' + match.id)));
  }

  count(): Observable<any> {
    return this.http.get<number>(this.scoreBoarServerUrl + '/count')
      .pipe(
        map(res => res),
        catchError(this.handleError('match count', [])));
  }

  countByGame(gameId: string): Observable<any> {
    return this.http.get<number>(this.scoreBoarServerUrl + '/countByGame?gameId=' + gameId)
      .pipe(
        map(res => res),
        catchError(this.handleError('match count by game', [])));
  }

  countByPlayer(playerId: string): Observable<any> {
    return this.http.get<number>(this.scoreBoarServerUrl + '/countByPlayer?playerId=' + playerId)
      .pipe(
        map(res => res),
        catchError(this.handleError('match count by player', [])));
  }
}
