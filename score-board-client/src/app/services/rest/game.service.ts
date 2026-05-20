import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {BaseRestService} from './base-rest.service';
import {Game} from '../../models/game';
import {NotificationService} from '../common/notification.service';

@Injectable({
  providedIn: 'root'
})
export class GameService extends BaseRestService {
  private scoreBoarServerUrl = environment.scoreBoarServerUrl + '/api/game'; // URL to web api

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  getAll(): Observable<any> {
    return this.http.get<Game>(this.scoreBoarServerUrl)
      .pipe(
        map(res => res),
        catchError(this.handleError('get games', [])));
  }

  get(id: string): Observable<any> {
    return this.http.get<Game>(this.scoreBoarServerUrl + '/' + id)
      .pipe(
        map(res => res),
        catchError(this.handleError('get game for ' + id, [])));
  }

  count(): Observable<any> {
    return this.http.get<number>(this.scoreBoarServerUrl + '/count')
      .pipe(
        map(res => res),
        catchError(this.handleError('game count', [])));
  }

  add(game: Game) {
    return this.http.post<Game>(this.scoreBoarServerUrl, game)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('add game ' + game.name)));
  }

  update(game: Game) {
    return this.http.put<Game>(this.scoreBoarServerUrl + '/' + game.id, game)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('update game ' + game.id)));
  }

  getGenres(): Observable<any> {
    return this.http.get<string[]>(this.scoreBoarServerUrl + '/genres')
      .pipe(
        map(res => res),
        catchError(this.handleError('game genres', [])));
  }

  getTypes(): Observable<any> {
    return this.http.get<string[]>(this.scoreBoarServerUrl + '/types')
      .pipe(
        map(res => res),
        catchError(this.handleError('game types', [])));
  }
}
