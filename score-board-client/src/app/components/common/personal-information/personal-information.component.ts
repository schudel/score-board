import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Player} from '../../../models/player';
import {FileSystemFileEntry, NgxFileDropEntry} from 'ngx-file-drop';
import {DomSanitizer, SafeResourceUrl} from '@angular/platform-browser';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {PlayerService} from '../../../services/rest/player.service';
import {first} from 'rxjs/operators';
import {NotificationService} from '../../../services/common/notification.service';
import {Ng2ImgMaxService} from 'ng2-img-max';

@Component({
  selector: 'app-perosnal-information',
  templateUrl: './personal-information.component.html',
  styleUrls: ['./personal-information.component.scss']
})
export class PersonalInformationComponent implements OnInit, OnDestroy {
  subscriptions = [];
  error = '';
  submitted = false;
  personalInfoForm: FormGroup;
  currentPlayer: Player;
  files: NgxFileDropEntry[] = [];
  base64textString = '';
  image: SafeResourceUrl;
  tempPlayer: Player;
  isImageChanged = false;

  constructor(private authenticationService: AuthenticationService,
              private playerService: PlayerService,
              private notificationService: NotificationService,
              private sanitizer: DomSanitizer,
              private ng2ImgMax: Ng2ImgMaxService) {
    this.personalInfoForm = new FormGroup({
      playerName: new FormControl('', Validators.required),
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      email: new FormControl('', Validators.email),
      roleName: new FormControl(''),
      registrationDate: new FormControl('')
    });
    this.form.roleName.disable();
    this.form.registrationDate.disable();
  }

  ngOnInit() {
    const subscription = this.authenticationService.currentPlayer.subscribe(x => {
      if (!x) {
        return;
      }
      this.currentPlayer = x;
      this.initForm();
    });
    this.subscriptions.push(subscription);
  }

  // convenience getter for easy access to form fields
  get form() { return this.personalInfoForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.personalInfoForm.invalid) {
      return;
    }
    // copy original current player
    const player: Player = Object.assign({}, this.currentPlayer);
    player.playerName = this.form.playerName.value;
    player.firstName = this.form.firstName.value;
    player.lastName = this.form.lastName.value;
    player.email = this.form.email.value;
    if (this.isImageChanged) {
      player.image = this.base64textString;
    }
    // copy new current player an store in temp object
    this.tempPlayer = Object.assign({}, player);
    this.subscriptions.push(this.playerService.update(player)
      .pipe(first())
      .subscribe(
        () => {
          this.notificationService.success($localize`:@@personalUpdatedText:Your Personal Information has been updated.`,
            $localize`:@@personalUpdatedTitle:Update Personal Information`);
          this.error = '';
          // set current player
          this.authenticationService.setCurrentPlayerValue(this.tempPlayer);
          this.initForm();
        },
        error => {
          this.error = error;
          this.notificationService.error($localize`:@@resizeImageFailedText:Resize Image failed` + ': ' + error,
            $localize`:@@resizeImageFailedText:Resize Image failed`);
        }));
  }

  initForm() {
    this.personalInfoForm.setValue({
      playerName: this.currentPlayer.playerName,
      firstName:  this.currentPlayer.firstName,
      lastName: this.currentPlayer.lastName,
      email: this.currentPlayer.email,
      roleName: this.currentPlayer.roleName,
      registrationDate: this.currentPlayer.registrationDate});
    if (this.currentPlayer.image) {
      this.image = this.sanitizer.bypassSecurityTrustResourceUrl(this.currentPlayer.image);
    }
  }

  dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    if (this.files.length > 1) {
      this.notificationService.error($localize`:@@onlyOneImageText:You can only upload one Image.`,
      $localize`:@@imageUploadFailedTitle:Image upload failed`);
    }

    for (const droppedFile of files) {
      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file) {
            this.subscriptions.push(this.ng2ImgMax.resizeImage(file, 150, 150).subscribe(
              result => {
                const reader = new FileReader();
                reader.onload = this.handleReaderLoaded.bind(this);
                reader.readAsBinaryString(result);
              },
              error => {
                console.error('resize image', error);
              }
            ));
          }
          this.base64textString = 'data:' + file.type + ';base64,';
        });
      }
    }
  }

  handleReaderLoaded(readerEvt) {
    const binaryString = readerEvt.target.result;
    this.base64textString = this.base64textString + btoa(binaryString);
    this.image = this.sanitizer.bypassSecurityTrustResourceUrl(this.base64textString);
    this.isImageChanged = true;
  }

  removeImage() {
    this.image = '';
    this.base64textString = '';
    this.isImageChanged = true;
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
