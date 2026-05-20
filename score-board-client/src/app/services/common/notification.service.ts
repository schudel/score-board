import {Injectable} from '@angular/core';
import {IndividualConfig, ToastrService} from 'ngx-toastr';
import {Notification} from '../../models/notification';
import {NotificationType} from '../../models/notificationType';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  localStorageKey = 'notifications';
  private notificationsSubject: BehaviorSubject<Array<Notification>>;
  public notifications: Observable<Array<Notification>>;

  constructor(private toastr: ToastrService) {
    this.notificationsSubject = new BehaviorSubject<Array<Notification>>(JSON.parse(localStorage.getItem(this.localStorageKey)));
    this.notifications = this.notificationsSubject.asObservable();
  }

  public get notificationsValue(): Array<Notification> {
    return this.notificationsSubject.value;
  }

  public setNotificationsValue(notifications: Array<Notification>) {
    // store notifications
    localStorage.setItem(this.localStorageKey, JSON.stringify(notifications));
    this.notificationsSubject.next(notifications);
  }

  show(message?: string, title?: string, override?: Partial<IndividualConfig>, type?: string) {
    let n = this.notificationsSubject.value;
    if (!n) {
        n = new Array<Notification>();
    }
    n.push(new Notification(message, title, NotificationType.Show));
    this.setNotificationsValue(n);
    this.toastr.show(message, title, override, type);
  }

  success(message?: string, title?: string, override?: Partial<IndividualConfig>) {
    let n = this.notificationsSubject.value;
    if (!n) {
      n = new Array<Notification>();
    }
    n.push(new Notification(message, title, NotificationType.Success));
    this.setNotificationsValue(n);
    this.toastr.success(message, title, override);
  }

  error(message?: string, title?: string, override?: Partial<IndividualConfig>) {
    let n = this.notificationsSubject.value;
    if (!n) {
      n = new Array<Notification>();
    }
    n.push(new Notification(message, title, NotificationType.Error));
    this.setNotificationsValue(n);
    this.toastr.error(message, title, override);
  }

  info(message?: string, title?: string, override?: Partial<IndividualConfig>) {
    let n = this.notificationsSubject.value;
    if (!n) {
      n = new Array<Notification>();
    }
    n.push(new Notification(message, title, NotificationType.Info));
    this.setNotificationsValue(n);
    this.toastr.info(message, title, override);
  }

  warning(message?: string, title?: string, override?: Partial<IndividualConfig>) {
    let n = this.notificationsSubject.value;
    if (!n) {
      n = new Array<Notification>();
    }
    n.push(new Notification(message, title, NotificationType.Warning));
    this.setNotificationsValue(n);
    this.toastr.warning(message, title, override);
  }

  remove(notification: Notification) {
    this.setNotificationsValue(this.notificationsValue.filter(n => {
      return n.id !== notification.id;
    }));
  }

  clearAll() {
    this.setNotificationsValue([]);
    this.clearLocalStorage();
  }

  clearLocalStorage(): void {
    // remove player from local storage to log player out
    localStorage.removeItem(this.localStorageKey);
  }
}
