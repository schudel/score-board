import {Injectable} from '@angular/core';
import {AuthenticationService} from '../services/rest/authentication.service';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available
    const currentPlayer = this.authenticationService.currentPlayerValue;
    if (currentPlayer && currentPlayer.token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentPlayer.token}`
        }
      });
    }
    return next.handle(request);
  }
}
