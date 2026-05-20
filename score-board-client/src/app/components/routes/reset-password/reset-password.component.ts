import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {ToastrService} from 'ngx-toastr';
import {first} from 'rxjs/operators';
import {ResetPassword} from '../../../models/resetPassword';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit, OnDestroy  {
  subscriptions = [];
  error = '';
  submitted = false;
  resetPasswordForm: FormGroup;
  passwordRequestId = '';

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private authenticationService: AuthenticationService,
              private toastr: ToastrService) {
    this.resetPasswordForm = new FormGroup({
      password: new FormControl('', Validators.minLength(8)),
      passwordConfirm: new FormControl('')
    });
    this.resetPasswordForm.setValidators(this.comparisonValidator());
  }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    if (routeParams == null || routeParams.id == null) {
      this.error = '';
      return;
    }
    this.passwordRequestId = routeParams.id;
  }

  // convenience getter for easy access to form fields
  get form() { return this.resetPasswordForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.resetPasswordForm.invalid) {
      return;
    }

    const resetPassword: ResetPassword = new ResetPassword();
    resetPassword.passwordRequestId = this.passwordRequestId;
    resetPassword.password = this.form.password.value;

    this.subscriptions.push(this.authenticationService.resetPassword(resetPassword)
      .pipe(first())
      .subscribe(
        () => {
          this.toastr.success($localize`:@@successfullyResetPassword:Successfully reset the Password`, $localize`:@@resetPassword:Reset Password`);
          this.router.navigate(['/login']);
        },
        error => {
          this.error = error;
        }));
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
