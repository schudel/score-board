import {Component, OnDestroy, OnInit} from '@angular/core';
import {NotificationService} from '../../../services/common/notification.service';
import {Notification} from '../../../models/notification';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit, OnDestroy {
  subscriptions = [];
  notifications: Array<Notification>;

  constructor(private notificationService: NotificationService) {
  }

  ngOnInit() {
    this.subscriptions.push(this.notificationService.notifications.subscribe(x => {
      if (!x) {
        return;
      }
      this.notifications = x.sort((a, b) => {
        // Turn your strings into dates, and then subtract them
        // to get a value that is either negative, positive, or zero.
        return Date.parse(b.dateTime.toString()) - Date.parse(a.dateTime.toString());
      });
    }));
  }

  clear() {
    this.notificationService.clearAll();
  }

  removeNotification(notification: Notification) {
    this.notificationService.remove(notification);
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
