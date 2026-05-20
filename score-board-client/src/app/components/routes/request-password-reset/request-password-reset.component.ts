import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {ToastrService} from 'ngx-toastr';
import {first} from 'rxjs/operators';

@Component({
  selector: 'app-request-password-reset',
  templateUrl: './request-password-reset.component.html',
  styleUrls: ['./request-password-reset.component.scss']
})
export class RequestPasswordResetComponent implements OnInit, OnDestroy  {
  subscriptions = [];
  error = '';
  submitted = false;
  requestPasswordResetForm: FormGroup;
  requested = false;

  constructor(private authenticationService: AuthenticationService,
              private toastr: ToastrService) {
    this.requestPasswordResetForm = new FormGroup({
      email: new FormControl('', Validators.email)
    });
  }

  ngOnInit() {
  }

  // convenience getter for easy access to form fields
  get form() { return this.requestPasswordResetForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.requestPasswordResetForm.invalid) {
      return;
    }

    const email = this.form.email.value;
    this.subscriptions.push(this.authenticationService.requestPasswordReset(email)
      .pipe(first())
      .subscribe(
        () => {
          this.requested = true;
          this.toastr.success($localize`:@@successfullySendPasswordResetEmail:Successfully sent a Password Reset Email`, $localize`:@@resetPassword:Reset Password`);
        },
        error => {
          this.error = error;
        }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
