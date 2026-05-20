import {Component, ElementRef, HostListener, NgZone, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ChatService} from '../../../services/rest/chat.service';
import {Chat} from '../../../models/chat';
import {ChatRoom} from '../../../models/chat-room';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {Player} from '../../../models/player';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ChatTypingState} from '../../../models/chat-typing-state';
import {DomSanitizer, SafeHtml} from '@angular/platform-browser';
import {DateHelper} from '../../../helpers/date-helper';
import {QuillEditorComponent} from 'ngx-quill';
import {Constants} from '../../../constants/constants';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  constructor(private chatService: ChatService,
              private ngZone: NgZone,
              private authenticationService: AuthenticationService,
              private sr: DomSanitizer) {
    this.sendMessageForm = new FormGroup({
      message: new FormControl('', Validators.required)
    });
    this.chatTypingStates = [];
    this.rooms = [];
    this.rooms.push(new ChatRoom('ScoreBoard', 'Welcome to ScoreBoard Chat'));
    this.currentRoom = 'ScoreBoard';
    this.modules = {
      // formula: true,
      // syntax: true,
      toolbar: this.toolbarOptions
    };
    this.skip = 0;
    this.messageAmount = 10;
  }

  // convenience getter for easy access to form fields
  get form() { return this.sendMessageForm.controls; }
  subscriptions = [];
  currentPlayer: Player;
  sendMessageForm: FormGroup;
  canSendMessage: boolean;
  chatTypingStates: ChatTypingState[];
  rooms: ChatRoom[];
  currentRoom: string;
  submitted = false;
  modules = {};
  currentTypingText: string;
  isTyping: boolean;
  showEmojiPicker = false;
  skip: number;
  messageAmount: number;

  toolbarOptions = {
    container: [
      ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
      ['blockquote', 'code-block'],

      [{ header: 1 }, { header: 2 }],               // custom button values
      [{ list: 'ordered'}, { list: 'bullet' }],
      [ 'formula', { script: 'sub'}, { script: 'super' }],      // superscript/subscript
      // [{ indent: '-1'}, { indent: '+1' }],          // outdent/indent
      // [{ direction: 'rtl' }],                         // text direction

      // [{ size: ['small', false, 'large', 'huge'] }],  // custom dropdown
      // [{ header: [1, 2, 3, 4, 5, 6, false] }],

      // [{ color: [] }, { background: [] }],          // dropdown with defaults from theme
      // [{ font: [] }],
      // [{ align: [] }],

      ['clean'],                                         // remove formatting button

      ['link', 'image', 'video']                         // link and image, video
    ]
  };

  @ViewChild('scrollToBottom') private myScrollContainer: ElementRef;

  @ViewChild('quillEditor') private quillEditor: QuillEditorComponent;

  static contains(c: ChatTypingState, cts: ChatTypingState[]): boolean {
    if (!c || !cts) {
      return false;
    }
    for (const state of cts) {
      if (c.playerId === state.playerId) {
        return true;
      }
    }
    return false;
  }

  static getTypingStateIndex(c: ChatTypingState, cts: ChatTypingState[]): number {
    if (!c || !cts) {
      return 0;
    }
    for (const state of cts) {
      if (c.playerId === state.playerId) {
        return cts.indexOf(state);
      }
    }
    return 0;
  }

  ngOnInit() {
    this.getChatEntriesFromDataBase(true);
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
    this.subscriptions.push(this.chatService.connectionEstablished.subscribe(() => {
      this.canSendMessage = true;
    }));
    this.subscriptions.push(this.chatService.messageReceived.subscribe((chat: Chat) => {
      this.ngZone.run(() => {
        const room = this.rooms.find(t => t.heading === chat.room);
        if (room) {
          room.messageHistory.push(
            new Chat(chat.playerId, chat.userName, chat.message, chat.room, new Date(chat.timeStamp.toString())));
          setTimeout( () => { this.scrollToBottom(); }, 200 );
        }
      });
    }));
    this.subscriptions.push(this.chatService.typingStateReceived.subscribe((typingState: ChatTypingState) => {
      this.ngZone.run(() => {
        const room = this.rooms.find(t => t.heading === typingState.room);
        if (room) {
          if (typingState.playerId === this.currentPlayer.id) {
            return;
          }
          if (ChatComponent.contains(typingState, this.chatTypingStates)) {
            this.chatTypingStates[ChatComponent.getTypingStateIndex(typingState, this.chatTypingStates)] = typingState;
          } else {
            this.chatTypingStates.push(typingState);
          }
          this.currentTypingText = '';
          for (const state of this.chatTypingStates) {
            if (state.isTyping) {
              this.currentTypingText += state.userName + ', ';
            }
          }
          if (this.currentTypingText) {
            this.currentTypingText = this.currentTypingText.substring(0, this.currentTypingText.length - 2) + ' ' + $localize`:@@isTypingText:is typing`;
          }
          setTimeout( () => { this.scrollToBottom(); }, 200 );
        }
      });
    }));
    if (this.quillEditor && this.quillEditor.quillEditor) {
      this.quillEditor.quillEditor.focus();
    }
  }

  getChatEntriesFromDataBase(scrollToBottom: boolean) {
    this.subscriptions.push(this.chatService.getSpecificEntries(this.messageAmount, this.skip).subscribe(c => {
      if (!c) {
        return;
      }
      for (const chat of c) {
        const room = this.rooms.find(t => t.heading === chat.room);
        if (room) {
          room.messageHistory.push(
            new Chat(chat.playerId, chat.userName, chat.message, chat.room, DateHelper.GetDate(chat.timeStamp.toString())));
        }
      }
      for (const room of this.rooms) {
        room.messageHistory = room.messageHistory.sort((a, b) => {
          // Turn your strings into dates, and then subtract them
          // to get a value that is either negative, positive, or zero.
          return Date.parse(a.timeStamp.toString()) - Date.parse(b.timeStamp.toString());
        });
      }
      if (scrollToBottom) {
        setTimeout( () => { this.scrollToBottom(); }, 200 );
      }
    }));
  }

  sendMessage() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.sendMessageForm.invalid) {
      return;
    }

    if (this.canSendMessage) {
      const chatMessage = new Chat();
      chatMessage.playerId = this.currentPlayer.id;
      chatMessage.room = this.currentRoom;
      chatMessage.userName = this.currentPlayer.playerName;
      let value = this.form.message.value;
      if (value.endsWith('<p><br></p>')) {
        value = value.substring(0, value.length - 11);
      }
      chatMessage.message = value; // this.form.message.value.replace(/(?:\r\n|\r|\n)/g, '<br>');
      // set typing state to false
      this.sendTypingState(false);
      this.chatService.sendChatMessage(chatMessage);

      this.sendMessageForm = new FormGroup({
        message: new FormControl('', Validators.required)
      });
    }
  }

  sendTypingState(isTyping: boolean) {
    if (this.canSendMessage) {
      if (this.isTyping !== isTyping)  {
        this.chatService.sendTypingState(
          new ChatTypingState(this.currentPlayer.id, this.currentPlayer.playerName, isTyping, this.currentRoom));
        this.isTyping = isTyping;
      }
    }
  }

  OnRoomChange(room) {
    this.currentRoom = room;
  }

  changedEditor(event) {
    if (event.source === 'silent') {
      return;
    }
    if (event.html) {
      this.sendTypingState(true);
    } else {
      this.sendTypingState(false);
    }
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      if (!event.shiftKey) {
        this.sendMessage();
      }
    }
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) {
      console.log(err);
    }
  }

  public getSaveHtml(html: string): SafeHtml {
    return this.sr.bypassSecurityTrustHtml(html);
  }

  onGiphyImageSelected($event: string) {
    this.sendMessageForm.setValue({
      message: this.form.message.value + $event
    });
    if (this.quillEditor && this.quillEditor.quillEditor) {
      this.quillEditor.quillEditor.focus();
    }
  }

  @HostListener('scroll', ['$event'])
  onScroll(event: any) {
    if (event.target.scrollTop === 0) {
      if (this.rooms && this.rooms.length > 0 && this.rooms[0].messageHistory) {
        this.skip = this.rooms[0].messageHistory.length;
      }
      this.getChatEntriesFromDataBase(false);
    }
  }

  toggleEmojiPicker() {
    this.showEmojiPicker = !this.showEmojiPicker;
  }

  addEmoji(event) {
    const text = `${event.emoji.native}`;
    this.sendMessageForm.setValue({
      message: this.form.message.value + text
    });
    this.showEmojiPicker = false;
    if (this.quillEditor && this.quillEditor.quillEditor) {
      this.quillEditor.quillEditor.focus();
    }
  }

  getDateFormat(): string {
    return Constants.DateFormat;
  }

  ngOnDestroy(): void {
    // set typing state to false
    this.sendTypingState(false);
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
