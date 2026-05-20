import { Injectable } from '@angular/core';
import {Observable, throwError} from 'rxjs';
import {NotificationService} from '../common/notification.service';

@Injectable({
  providedIn: 'root'
})
export class BaseRestService {

  constructor(public notificationService: NotificationService) { }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  public handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(operation + ': ' + error);
      this.notificationService.error(error, 'Error');
      // Let the app keep running by returning an empty result.
      return throwError(error);
    };
  }
/*
  public handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  };

 */
}
