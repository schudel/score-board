import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {first} from 'rxjs/operators';
import {ThemeService} from '../../../services/common/theme.service';
// import {ReCaptchaV3Service} from 'ng-recaptcha';
// import {RecaptchaVerifyService} from '../../../services/rest/recaptcha-verify.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  subscriptions = [];
  returnUrl: string;
  error = '';
  // token: string;
  isDarkTheme: boolean;
  loginForm: FormGroup = new FormGroup({
    playerName: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    stayLoggedIn: new FormControl(''),
  });
  currentYear = (new Date()).getFullYear();

  constructor(private route: ActivatedRoute,
              private router: Router,
              private authenticationService: AuthenticationService,
              // private recaptchaV3Service: ReCaptchaV3Service,
              // private recaptchaVerifyService: RecaptchaVerifyService,
              private themeService: ThemeService,
              ) {
    // redirect to home if already logged in
    if (this.authenticationService.currentPlayerValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    // reset login status
    // this.authenticationService.logout();

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams.returnUrl || '/';
    this.subscriptions.push(this.themeService.isDarkTheme.subscribe(x => this.isDarkTheme  = x));
  }

  // convenience getter for easy access to form fields
  get form() { return this.loginForm.controls; }

  onSubmit() {
    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    // this.subscriptions.push(this.recaptchaV3Service.execute('login').subscribe((token) => {
      // this.token = token;
      /*
      this.subscriptions.push(
        this.recaptchaVerifyService.verifyToken(token).subscribe(x => {
          if (!x) {
            this.error = 'Recaptcha verification failed! Are you a Robot? If not, please refresh Site and try again.';
            return;
          }
        }));
*/
    this.subscriptions.push(this.authenticationService.login(this.form.playerName.value,
        this.form.password.value, this.form.stayLoggedIn.value)
        .pipe(first())
        .subscribe(
          data => {
            this.router.navigate([this.returnUrl]);
          },
          error => {
            this.error = error;
          }));

    // }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
