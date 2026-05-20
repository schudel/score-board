import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BaseRestService} from './base-rest.service';
import {environment} from '../../../environments/environment';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {Rating} from '../../models/rating';
import {NotificationService} from '../common/notification.service';
import {RatingHistory} from '../../models/ratingHistory';

@Injectable({
  providedIn: 'root'
})
export class RatingService extends BaseRestService {
  private scoreBoarServerUrl = environment.scoreBoarServerUrl + '/api/rating'; // URL to web api

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
  }

  getAll(slim: boolean = true): Observable<any> {
    return this.http.get<Rating>(this.scoreBoarServerUrl + '?slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get ratings', [])));
  }

  get(id: string, slim: boolean = false): Observable<any> {
    return this.http.get<Rating>(this.scoreBoarServerUrl + '/' + id + '?slim=' + slim)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get rating for ' + id, [])));
  }

  getByPlayer(playerId: string): Observable<any> {
    return this.http.get<Rating>(this.scoreBoarServerUrl + '/player?playerId=' + playerId)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get ratings for player id ' + playerId, [])));
  }

  calcAllRatingsAndSave() {
    return this.http.put<Rating>(this.scoreBoarServerUrl + '/calcAllAndSave', '')
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('calc all and save', [])));
  }

  calcAllMatchQualitiesAndSave() {
    return this.http.put<Rating>(this.scoreBoarServerUrl + '/calcAllMatchQualitiesAndSave', '')
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('calc all match qualities and save', [])));
  }

  getAllRatingHistories(): Observable<any> {
    return this.http.get<RatingHistory[]>(this.scoreBoarServerUrl + '/getAllRatingHistories')
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get All RatingHistories', [])));
  }

  calcAllRatingHistories(): Observable<any> {
    return this.http.get<RatingHistory[]>(this.scoreBoarServerUrl + '/calcAllRatingHistories')
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('calc All RatingHistories', [])));
  }

  calcAllRatingHistoriesAndSave(): Observable<any> {
    return this.http.get<RatingHistory[]>(this.scoreBoarServerUrl + '/calcAllRatingHistoriesAndSave')
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('calc All RatingHistories and save', [])));
  }
}
