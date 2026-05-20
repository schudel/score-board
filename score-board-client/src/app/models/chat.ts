import {Guid} from '../helpers/guid';

export class Chat {
  id: string;
  playerId: string;
  userName: string;
  message: string;
  room: string;
  timeStamp: Date;

  constructor(playerId: string = '', userName: string = '', message: string = '', room: string = '', timeStamp: Date = new Date()) {
    this.id = Guid.newGuid().ToString();
    this.playerId = playerId;
    this.userName = userName;
    this.message = message;
    this.room = room;
    this.timeStamp = timeStamp;
  }
}
