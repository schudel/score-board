import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {PlayerService} from '../../../services/rest/player.service';
import {Player} from '../../../models/player';
import {ChangePassword} from '../../../models/changePassword';
import {first} from 'rxjs/operators';
import {NotificationService} from '../../../services/common/notification.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit, OnDestroy  {
  subscriptions = [];
  changePasswordForm: FormGroup;
  error = '';
  submitted = false;
  currentPlayer: Player;

  constructor(private authenticationService: AuthenticationService,
              private playerService: PlayerService,
              private notificationService: NotificationService) {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', Validators.minLength(8)),
      confirmPassword: new FormControl('', Validators.required)
    });
    this.changePasswordForm.setValidators(this.comparisonValidator());
  }

  ngOnInit() {
    const subscription = this.authenticationService.currentPlayer.subscribe(x => {
      if (!x) {
        return;
      }
      this.currentPlayer = x;
    });
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
  }

  // convenience getter for easy access to form fields
  get form() { return this.changePasswordForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.changePasswordForm.invalid) {
      return;
    }
    const changePassword: ChangePassword = new ChangePassword();
    changePassword.currentPassword = this.form.currentPassword.value;
    changePassword.newPassword = this.form.newPassword.value;
    this.playerService.changePassword(this.currentPlayer.id, changePassword)
      .pipe(first())
      .subscribe(
        () => {
          this.notificationService.success($localize`:@@passwordUpdatedText:Your Password has been successfully updated.`,
          $localize`:@@passwordUpdatedTitle:Update Password successful`);
          this.error = '';
        },
        error => {
          this.error = error;
        });
  }

  public comparisonValidator(): ValidatorFn {
    return (group: FormGroup): ValidationErrors => {
      const control1 = group.controls.newPassword;
      const control2 = group.controls.confirmPassword;
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
