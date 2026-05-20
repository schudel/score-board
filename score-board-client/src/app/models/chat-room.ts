import {Chat} from './chat';

export class ChatRoom {
  messageHistory: Chat[];
  heading: string;
  title: string;

  constructor(heading: string = '', title: string = '')  {
    this.heading = heading;
    this.title = title;
    this.messageHistory = [];
  }
}
