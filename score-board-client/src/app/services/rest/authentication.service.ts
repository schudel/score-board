import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import {BehaviorSubject, Observable} from 'rxjs';
import {Player} from '../../models/player';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {catchError, map} from 'rxjs/operators';
import {BaseRestService} from './base-rest.service';
import {Role} from '../../models/role';
import {NotificationService} from '../common/notification.service';
import {ThemeService} from '../common/theme.service';
import {LiveMatchService} from './live-match.service';
import {ResetPassword} from '../../models/resetPassword';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService extends BaseRestService {
  private authenticationUrl = environment.scoreBoarServerUrl + '/api/authentication'; // URL to web api
  localStorageKey = 'currentPlayer';

  private currentPlayerSubject: BehaviorSubject<Player>;
  public currentPlayer: Observable<Player>;

  constructor(private http: HttpClient,
              public notificationService: NotificationService,
              private themeService: ThemeService,
              private liveMatchService: LiveMatchService) {
    super(notificationService);

    this.currentPlayerSubject = new BehaviorSubject<Player>(JSON.parse(localStorage.getItem(this.localStorageKey)));
    this.currentPlayer = this.currentPlayerSubject.asObservable();
  }

  public get currentPlayerValue(): Player {
    return this.currentPlayerSubject.value;
  }

  public setCurrentPlayerValue(player: Player) {
    // store player details and jwt token in local storage to keep player logged in between page refreshes
    localStorage.setItem(this.localStorageKey, JSON.stringify(player));
    this.currentPlayerSubject.next(player);
  }

  login(playerName: string, password: string, stayLoggedIn: boolean) {
    return this.http.post<Player>(this.authenticationUrl + '/authenticate', { playerName, password })
      .pipe(
        map(player => {
          // login successful if there's a jwt token in the response
          if (player && player.token) {
            player.role = Role[player.roleName];
            this.setCurrentPlayerValue(player);
            // apply user settings
            this.themeService.setDarkTheme(player.settings.darkMode);
          }
          return player;
        }),
        catchError(this.handleError('authentication', [])));
  }

  register(player: Player) {
    return this.http.post<any>(this.authenticationUrl + '/Register', player)
      .pipe(
        map(() => { console.log('register successful'); }),
        catchError(this.handleError('register', [])));
  }

  activate(id: string) {
    return this.http.get<any>(this.authenticationUrl + '/activate/' + id)
      .pipe(
        map(() => { console.log('activate successful'); }),
        catchError(this.handleError('activate', [])));
  }

  resendEmail(id: string) {
    return this.http.get<any>(this.authenticationUrl + '/resendemail/' + id)
      .pipe(
        map(() => { console.log('resend email successful'); }),
        catchError(this.handleError('resend email', [])));
  }

  requestPasswordReset(email: string) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const json = JSON.stringify(email);
    return this.http.post(this.authenticationUrl + '/RequestPasswordReset', json, {headers})
      .pipe(
        map(() => { console.log('Request Password Reset successful'); }),
        catchError(this.handleError('Request Password Reset', [])));
  }

  resetPassword(resetPassword: ResetPassword) {
    return this.http.put<ResetPassword>(this.authenticationUrl + '/ResetPassword', resetPassword)
      .pipe(
        map(() => { console.log('Reset Password successful'); }),
        catchError(this.handleError('Reset Password Reset', [])));
  }

  logout() {
    // clear local storage's
    this.notificationService.clearLocalStorage();
    this.themeService.clearLocalStorage();
    this.liveMatchService.clearLocalStorage();
    // remove player from local storage to log player out
    localStorage.removeItem(this.localStorageKey);
    this.currentPlayerSubject.next(null);
    // this.router.navigate(['/login']);
    console.log('logout successful');
    location.reload();
  }
}
