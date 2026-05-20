import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormControl} from '@angular/forms';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit, OnDestroy {
  subscriptions = [];
  selected = new FormControl(0);

  constructor(private activeRoute: ActivatedRoute) {
  }

  ngOnInit() {
    this.subscriptions.push(this.activeRoute.queryParams.subscribe (
      params => {
        const tab = params.tab || '';
        if (!tab || tab === '' || tab === 'personal-information') {
          this.selected.setValue(0);
        } else if (tab === 'change-password') {
          this.selected.setValue(1);
        } else if (tab === 'profile-settings') {
          this.selected.setValue(2);
        }
      }
    ));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
