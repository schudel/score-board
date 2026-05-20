import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {first} from 'rxjs/operators';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Player} from '../../../models/player';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
// import {ReCaptchaV3Service} from 'ng-recaptcha';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit, OnDestroy {
  subscriptions = [];
  error = '';
  submitted = false;
  registerForm: FormGroup;
  // token: string;

  constructor(private router: Router,
              private authenticationService: AuthenticationService,
              // private recaptchaV3Service: ReCaptchaV3Service,
              private toastr: ToastrService) {
    this.registerForm = new FormGroup({
      playerName: new FormControl('', Validators.required),
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      email: new FormControl('', Validators.email),
      password: new FormControl('', Validators.minLength(8)),
      passwordConfirm: new FormControl('')
    });
    this.registerForm.setValidators(this.comparisonValidator());
  }

  ngOnInit() {

  }

  // convenience getter for easy access to form fields
  get form() { return this.registerForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    // this.subscriptions.push(this.recaptchaV3Service.execute('login').subscribe((token) => {
      // this.token = token;
      // TODO: handle token

    const player: Player = new Player();
    player.playerName = this.form.playerName.value;
    player.firstName = this.form.firstName.value;
    player.lastName = this.form.lastName.value;
    player.email = this.form.email.value;
    player.password = this.form.password.value;

    this.subscriptions.push(this.authenticationService.register(player)
        .pipe(first())
        .subscribe(
          () => {
            this.toastr.success($localize`:@@registrationHiText:Hi` + ' ' + player.playerName +
              ', ' + $localize`:@@registrationSuccessfulText:your registration was successful! Please confirm the Email you just received to complete the activation process.`,
              $localize`:@@registrationSuccessfulTitle:Registration successful`);
            this.router.navigate(['/login']).then(() => {});
          },
          error => {
            this.error = error;
          }));
    // }));
  }

  public comparisonValidator(): ValidatorFn {
    return (group: FormGroup): ValidationErrors => {
      const control1 = group.controls.password;
      const control2 = group.controls.passwordConfirm;
      if (control1.value !== control2.value) {
        control2.setErrors({notEquivalent: true});
      } else {
        control2.setErrors(null);
      }
      return;
    };
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}

