import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';
import {LoadingScreenService} from '../../../services/common/loading-screen.service';
import {debounceTime} from 'rxjs/operators';

@Component({
  selector: 'app-loading-screen',
  templateUrl: './loading-screen.component.html',
  styleUrls: ['./loading-screen.component.scss']
})
export class LoadingScreenComponent implements OnInit, OnDestroy {
  loading = false;
  loadingSubscription: Subscription;

  constructor(private loadingScreenService: LoadingScreenService) { }

  ngOnInit() {
    this.loading = this.loadingScreenService.loading;
    this.loadingSubscription = this.loadingScreenService.loadingStatus.pipe(
      debounceTime(200)
    ).subscribe((value) => {
      this.loading = value as boolean;
    });
  }

  ngOnDestroy() {
    if (this.loadingSubscription !== undefined) {
      this.loadingSubscription.unsubscribe();
    }
  }
}
