import {Component, Inject, LOCALE_ID} from '@angular/core';
import {environment} from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Scoreboard';
  localStorageKey = 'currentPlayer';

  constructor(@Inject(LOCALE_ID) public locale: string) {
    for (const language of environment.languages) {
      if (window.location.pathname.startsWith('/' + language +'/') || window.location.pathname.startsWith('/' + language)) {
        return
      }
    }

    let localeId = locale.toLowerCase();
    const player = JSON.parse(localStorage.getItem(this.localStorageKey));
    if (player && player.settings && player.settings.languageCode) {
      localeId = player.settings.languageCode.toLowerCase();
      window.location.href = environment.scoreBoardRedirectUrl + localeId + '/';
      return;
    }

    let unknown = true;
    for (const language of environment.languages) {
      if(localeId === language) {
        unknown = false;
        break;
      }
    }
    if (unknown) {
      localeId = 'en-us';
      window.location.href = environment.scoreBoardRedirectUrl + localeId + '/';
    }
  }
}
