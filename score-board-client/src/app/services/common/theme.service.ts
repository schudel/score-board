import {Inject, Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {DOCUMENT} from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  localStorageKey = 'theme';
  private darkThemeSubject: BehaviorSubject<boolean>;
  public isDarkTheme: Observable<boolean>;

  constructor(@Inject(DOCUMENT) private document: Document) {
    this.darkThemeSubject = new BehaviorSubject<boolean>(JSON.parse(localStorage.getItem(this.localStorageKey)));
    this.isDarkTheme = this.darkThemeSubject.asObservable();
    if (this.getDarkThemeValue) {
      this.document.body.classList.add('dark-theme');
    } else {
      this.document.body.classList.remove('dark-theme');
    }
  }

  public get getDarkThemeValue(): boolean {
    return this.darkThemeSubject.value;
  }

  setDarkTheme(isDarkTheme: boolean): void {
    if (isDarkTheme) {
      this.document.body.classList.add('dark-theme');
    } else {
      this.document.body.classList.remove('dark-theme');
    }
    // store theme
    localStorage.setItem(this.localStorageKey, JSON.stringify(isDarkTheme));
    this.darkThemeSubject.next(isDarkTheme);
  }

  clearLocalStorage(): void {
    // remove player from local storage to log player out
    localStorage.removeItem(this.localStorageKey);
  }
}
