import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {first} from 'rxjs/operators';

@Component({
  selector: 'app-activation',
  templateUrl: './activation.component.html',
  styleUrls: ['./activation.component.scss']
})
export class ActivationComponent implements OnInit, OnDestroy {
  subscriptions = [];
  activationSuccessful: boolean;
  error: string;
  private playerId: string;

  constructor(private activeRoute: ActivatedRoute,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    if (routeParams == null || routeParams.id == null) {
      this.error = '';
      this.activationSuccessful = false;
      return;
    }
    this.playerId = routeParams.id;
    const subscription = this.authenticationService.activate(this.playerId)
      .pipe(first())
      .subscribe(
        data => {
          this.activationSuccessful = true;
        },
        error => {
          this.error = error;
        });
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
  }

  resendEmail() {
    const subscription = this.authenticationService.resendEmail(this.playerId)
      .pipe(first())
      .subscribe(
        data => {

        },
        error => {
          this.error = error;
        });
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
