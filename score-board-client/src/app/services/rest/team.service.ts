import { Injectable } from '@angular/core';
import {BaseRestService} from './base-rest.service';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {NotificationService} from '../common/notification.service';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {Team} from '../../models/team';

@Injectable({
  providedIn: 'root'
})
export class TeamService extends BaseRestService {
  private scoreBoarServerUrl = environment.scoreBoarServerUrl + '/api/team'; // URL to web api

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  getAll(): Observable<any> {
    return this.http.get<Team>(this.scoreBoarServerUrl)
      .pipe(
        map(res => res),
        catchError(this.handleError('get teams', [])));
  }

  get(id: string): Observable<any> {
    return this.http.get<Team>(this.scoreBoarServerUrl + '/' + id)
      .pipe(
        map(res => res),
        catchError(this.handleError('get team for ' + id, [])));
  }

  getNames(): Observable<any> {
    return this.http.get<string[]>(this.scoreBoarServerUrl + '/names')
      .pipe(
        map(res => res),
        catchError(this.handleError('team names', [])));
  }
}
