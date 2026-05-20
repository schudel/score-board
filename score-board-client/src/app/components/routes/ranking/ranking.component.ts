import {Component, OnDestroy, OnInit} from '@angular/core';
import {RatingService} from '../../../services/rest/rating.service';
import {Rating} from '../../../models/rating';
import {RatingHelperService} from '../../../services/common/rating-helper.service';

@Component({
  selector: 'app-ranking',
  templateUrl: './ranking.component.html',
  styleUrls: ['./ranking.component.scss']
})
export class RankingComponent implements OnInit, OnDestroy {
  subscriptions = [];
  ratings: Rating[];

  constructor(private ratingService: RatingService) { }

  ngOnInit() {
    this.subscriptions.push(this.ratingService.getAll().subscribe(
      r => {
        this.ratings = r.sort(RatingHelperService.sortByConservativeRating);
      }
    ));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
