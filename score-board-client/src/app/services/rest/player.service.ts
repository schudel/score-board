import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {environment} from '../../../environments/environment';
import {BaseRestService} from './base-rest.service';
import {HttpClient} from '@angular/common/http';
import {Player} from '../../models/player';
import {ChangePassword} from '../../models/changePassword';
import {Settings} from '../../models/settings';
import {NotificationService} from '../common/notification.service';

@Injectable({
  providedIn: 'root'
})
export class PlayerService extends BaseRestService {
  private scoreBoarServerUrl = environment.scoreBoarServerUrl + '/api/player'; // URL to web api

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  getAll(): Observable<any> {
    return this.http.get<Player>(this.scoreBoarServerUrl)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get players', [])));
  }

  get(id: string): Observable<any> {
    return this.http.get<Player>(this.scoreBoarServerUrl + '/' + id)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get player for ' + id, [])));
  }

  update(player: Player) {
    // delete data we won't transfer to server
    delete player.role;
    delete player.password;
    return this.http.put<Player>(this.scoreBoarServerUrl + '/' + player.id, player)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('update player ' + player.id)));
  }

  changePassword(id: string, changePassword: ChangePassword) {
    return this.http.put<Player>(this.scoreBoarServerUrl + '/changePassword/' + id, changePassword)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('change password: ' + id)));
  }

  updateSettings(id: string, settings: Settings) {
    return this.http.put<Player>(this.scoreBoarServerUrl + '/settings/' + id, settings)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('change password: ' + id)));
  }

  count(): Observable<any> {
    return this.http.get<number>(this.scoreBoarServerUrl + '/count')
      .pipe(
        map(res => res),
        catchError(this.handleError('player count', [])));
  }
}
