import {EventEmitter, Injectable} from '@angular/core';
import {Chat} from '../../models/chat';
import {HubConnection, HubConnectionBuilder} from '@aspnet/signalr';
import {environment} from '../../../environments/environment';
import {ChatTypingState} from '../../models/chat-typing-state';
import {BaseRestService} from './base-rest.service';
import {HttpClient} from '@angular/common/http';
import {NotificationService} from '../common/notification.service';
import {Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ChatService extends BaseRestService {
  private scoreBoarServerUrlWebSocket = environment.scoreBoarServerUrl + '/chat'; // URL to websocket
  private scoreBoarServerUrlWebApi = environment.scoreBoarServerUrl + '/api/chat'; // URL to web api
  private connectionIsEstablished = false;
  private hubConnection: HubConnection;

  messageReceived = new EventEmitter<Chat>();
  typingStateReceived = new EventEmitter<ChatTypingState>();
  connectionEstablished = new EventEmitter<boolean>();

  constructor(private http: HttpClient, public notificationService: NotificationService) {
    super(notificationService);
    if (!this.connectionIsEstablished) {
      this.createConnection();
      this.registerOnServerEvents();
      this.startConnection();
    }
  }

  IsConnectionEstablished(): boolean {
    return this.connectionIsEstablished;
  }

  sendChatMessage(message: Chat) {
    this.hubConnection.invoke('SendMessage', message)
      .then()
      .catch(err => console.error(err.toString()));
  }

  sendTypingState(typingState: ChatTypingState) {
    this.hubConnection.invoke('SendTypingState', typingState)
      .then()
      .catch(err => console.error(err.toString()));
  }

  getAll(): Observable<any> {
    return this.http.get<Chat>(this.scoreBoarServerUrlWebApi)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get chats', [])));
  }

  getSpecificEntries(amount: number, skip: number): Observable<any> {
    return this.http.get<Chat>(this.scoreBoarServerUrlWebApi + '/limit?amount=' + amount + '&skip=' + skip)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get amount of ' + amount + ' chats and skip the first ' + skip + ' entries', [])));
  }

  get(id: string): Observable<any> {
    return this.http.get<Chat>(this.scoreBoarServerUrlWebApi + '/' + id )
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get chat for ' + id, [])));
  }

  getByPlayer(playerId: string): Observable<any> {
    return this.http.get<Chat>(this.scoreBoarServerUrlWebApi + '/player?playerId=' + playerId)
      .pipe(
        map(res => res),
        catchError(this.handleError<any>('get chats for player id ' + playerId, [])));
  }

  deleteAll() {
    return this.http.delete<Chat>(this.scoreBoarServerUrlWebApi)
      .pipe(
        map(() => {}),
        catchError(this.handleError<any>('delete all chats', [])));
  }

  private createConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.scoreBoarServerUrlWebSocket)
      .build();
  }

  private startConnection(): void {
    this.hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Chat Hub connection started');
        this.connectionEstablished.emit(true);
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
        setTimeout(this.startConnection, 1000);
      });
  }

  private registerOnServerEvents(): void {
    this.hubConnection.on('ReceiveMessage', (data: any) => {
      this.messageReceived.emit(data);
    });
    this.hubConnection.on('ReceiveTypingState', (data: any) => {
      this.typingStateReceived.emit(data);
    });
  }
}
