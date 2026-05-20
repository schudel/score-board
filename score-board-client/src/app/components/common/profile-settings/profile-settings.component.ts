import {Component, OnDestroy, OnInit} from '@angular/core';
import {ThemeService} from '../../../services/common/theme.service';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Player} from '../../../models/player';
import {first} from 'rxjs/operators';
import {PlayerService} from '../../../services/rest/player.service';
import {NotificationService} from '../../../services/common/notification.service';
import {Settings} from '../../../models/settings';
import {Language} from '../../../models/language';
import {environment} from '../../../../environments/environment';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.scss']
})
export class ProfileSettingsComponent implements OnInit, OnDestroy {
  subscriptions = [];
  settingsForm: FormGroup;
  error = '';
  submitted = false;
  currentPlayer: Player;
  languages: Array<Language>;

  constructor(private authenticationService: AuthenticationService,
              private playerService: PlayerService,
              private notificationService: NotificationService,
              private themeService: ThemeService) {
    this.settingsForm = new FormGroup({
      languageCode: new FormControl('', Validators.required),
      darkMode: new FormControl(''),
    });
    this.languages = Language.getLanguages();
  }

  ngOnInit() {
    const subscription = this.authenticationService.currentPlayer.subscribe(x => {
      if (!x) {
        return;
      }
      this.currentPlayer = x;
      if (this.currentPlayer.settings) {
        this.settingsForm.setValue({
          languageCode: this.currentPlayer.settings.languageCode,
          darkMode:  this.currentPlayer.settings.darkMode});
      }
    });
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
  }

  // convenience getter for easy access to form fields
  get form() { return this.settingsForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.settingsForm.invalid) {
      return;
    }
    const settings: Settings = new Settings();
    settings.id = this.currentPlayer.settings.id;
    settings.languageCode = this.form.languageCode.value;
    settings.darkMode = this.form.darkMode.value;
    settings.dashboardLayout = this.currentPlayer.settings.dashboardLayout;
    const tempLanguageCode = this.currentPlayer.settings.languageCode;
    this.playerService.updateSettings(this.currentPlayer.id, settings)
      .pipe(first())
      .subscribe(
        () => {
          this.notificationService.success($localize`:@@settingsUpdatedText:Your Settings has been successfully updated.`,
            $localize`:@@settingsUpdatedTitle:Update Settings`);
          this.error = '';
          this.currentPlayer.settings = settings;
          this.authenticationService.setCurrentPlayerValue(this.currentPlayer);
          this.themeService.setDarkTheme(this.currentPlayer.settings.darkMode);
          if (this.currentPlayer.settings.languageCode !== tempLanguageCode) {
            // redirect to locale version
            // profile?tab=profile-settings
            window.location.href = environment.scoreBoardRedirectUrl + this.currentPlayer.settings.languageCode + '/';
          }
        },
        error => {
          this.error = error;
        });
  }

  toggleDarkTheme(checked: boolean) {
    this.themeService.setDarkTheme(checked);
  }

  ngOnDestroy(): void {
    // set back dark mode
    if (this.themeService.getDarkThemeValue !== this.currentPlayer.settings.darkMode) {
      this.notificationService.warning($localize`:@@darkThemeSettingsResetText:Dark-Theme Settings were reset because the Settings were not saved.`, $localize`:@@darkThemeSettingsResetTitle:Dark-Theme Settings`);
      this.themeService.setDarkTheme(this.currentPlayer.settings.darkMode);
    }
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
