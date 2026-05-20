export class ChatTypingState {
  playerId: string;
  userName: string;
  isTyping: boolean;
  room: string;
  timeStamp: Date;

  constructor(playerId: string = '', userName: string = '', isTyping: boolean = false, room: string = '') {
    this.playerId = playerId;
    this.userName = userName;
    this.isTyping = isTyping;
    this.room = room;
    this.timeStamp = new Date();
  }
}
