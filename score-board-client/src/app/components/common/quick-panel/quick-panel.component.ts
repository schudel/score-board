import {Component, OnDestroy, OnInit} from '@angular/core';
import {NotificationService} from '../../../services/common/notification.service';

@Component({
  selector: 'app-quick-panel',
  templateUrl: './quick-panel.component.html',
  styleUrls: ['./quick-panel.component.scss']
})
export class QuickPanelComponent implements OnInit, OnDestroy {
  subscriptions = [];
  notificationsCount: number;

  constructor(private notificationService: NotificationService) { }

  ngOnInit() {
    const subscription = this.notificationService.notifications.subscribe(x => {
      if (x) {
        this.notificationsCount  = x.length;
      }
    });
    // manually keep track of the subscriptions in a subscription array
    this.subscriptions.push(subscription);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
