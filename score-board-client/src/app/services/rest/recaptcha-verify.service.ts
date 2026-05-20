import {Injectable} from '@angular/core';
import {Observable, of as observableOf} from 'rxjs';

// TODO: reCAPTCHA token verification must be performed server-side.
// The backend should expose an endpoint that accepts the token and verifies it
// against Google's siteverify API using the SECRET key (never expose the secret key
// in frontend code). This service should call that backend endpoint instead.
// See: https://developers.google.com/recaptcha/docs/verify

@Injectable({
  providedIn: 'root'
})
export class RecaptchaVerifyService {

  verifyToken(token: string): Observable<boolean> {
    // TODO: Replace with a call to the backend verification endpoint, e.g.:
    // return this.http.post<boolean>(`${environment.apiUrl}/api/authentication/verify-captcha`, { token });
    return observableOf(!!token);
  }
}
