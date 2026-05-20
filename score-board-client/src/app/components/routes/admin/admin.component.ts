import {Component, OnDestroy, OnInit} from '@angular/core';
import {RatingService} from '../../../services/rest/rating.service';
import {NotificationService} from '../../../services/common/notification.service';
import { MatDialog } from '@angular/material/dialog';
import {ModalDialogComponent} from '../../common/modal-dialog/modal-dialog.component';
import {ChatService} from '../../../services/rest/chat.service';
import {LiveMatchService} from '../../../services/rest/live-match.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit, OnDestroy {
  subscriptions = [];

  constructor(private ratingService: RatingService,
              private notificationService: NotificationService,
              private chatService: ChatService,
              private liveMatchService: LiveMatchService,
              public dialog: MatDialog) { }

  ngOnInit() {
  }

  calcAllRatings() {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@reCalcAllRatingsTitle:Re-Calc all Ratings`,
        message: $localize`:@@reCalcAllRatingsText:Are you sure you want to Re-Calc all Ratings? This could take some time.`}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        this.subscriptions.push(this.ratingService.calcAllRatingsAndSave().pipe().subscribe(() => {
          this.notificationService.success($localize`:@@allRatingsRecalculatedText:All Ratings has been successfully recalculated.`,
            $localize`:@@allRatingsRecalculatedTitle:Recalculate Ratings successful`);
        }));
      }
    }));
  }

  calcAllMatchQualities() {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@allMatchQualitiesRecalculatedTitle:Recalculate all Match Qualities`,
        message: $localize`:@@allMatchQualitiesRecalculatedText:Are you sure you want to recalculate all Match Qualities? This could take some time.`}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        this.subscriptions.push(this.ratingService.calcAllMatchQualitiesAndSave().pipe().subscribe(() => {
          this.notificationService.success($localize`:@@allMatchQualitiesRecalculatedSuccessfullyText:All Match Qualities has been successfully recalculated.`,
            $localize`:@@allMatchQualitiesRecalculatedSuccessfullyTitle:Recalculate Match Qualities successful`);
        }));
      }
    }));
  }

  deleteAllChats() {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@deleteAllChatMessagesTitle:Delete all Chat Messages`,
        message: $localize`:@@deleteAllChatMessagesText:Are you sure you want to delete all Chat Messages? This could take some time.`}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        this.subscriptions.push(this.chatService.deleteAll().pipe().subscribe(() => {
          this.notificationService.success($localize`:@@allChatMessagesDeletedText:All Chat Messages has been successfully deleted.`,
            $localize`:@@allChatMessagesDeletedTitle:Delete Chat Messages successful`);
        }));
      }
    }));
  }

  deleteAllLiveMatches() {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@allLiveMatchesDeletedTitle:Delete all Live Matches`,
        message: $localize`:@@allLiveMatchesDeletedText:Are you sure you want to delete all Live Matches? Note: the State of the normal Match will no be changed.`}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        this.subscriptions.push(this.liveMatchService.deleteAll().pipe().subscribe(() => {
          this.notificationService.success($localize`:@@allLiveMatchesDeletedSuccessfullyText:All Live Matches has been successfully deleted.`,
            $localize`:@@allLiveMatchesDeletedSuccessfullyTitle:Delete Live Matches successful`);
        }));
      }
    }));
  }

  calcAllRatingHistories() {
    const dialogRef = this.dialog.open(ModalDialogComponent, {
      width: '350px',
      data: {title: $localize`:@@allReCalcRatingHistoriesTitle:Recalculate all Rating Histories`,
        message: $localize`:@@allReCalcRatingHistoriesText:Are you sure you want to recalculate all Rating Histories? This could take some time.`}
    });

    this.subscriptions.push(dialogRef.afterClosed().subscribe(result => {
      if (result === 'ok') {
        this.subscriptions.push(this.ratingService.calcAllRatingHistoriesAndSave().pipe().subscribe(() => {
          this.notificationService.success($localize`:@@allReCalcRatingHistoriesSuccessfullyText:Recalculate all Rating Histories`,
            $localize`:@@allReCalcRatingHistoriesSuccessfullyTitle:Recalculate Rating Histories successful`);
        }));
      }
    }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
