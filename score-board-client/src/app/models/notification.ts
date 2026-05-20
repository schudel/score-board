import {NotificationType} from './notificationType';
import {Guid} from '../helpers/guid';

export class Notification {
  id: string;
  message: string;
  title: string;
  type: NotificationType;
  dateTime: Date;

  constructor(message: string, title: string, type: NotificationType) {
    this.id = Guid.newGuid().ToString();
    this.message = message;
    this.title = title;
    this.type = type;
    this.dateTime = new Date();
  }
}
