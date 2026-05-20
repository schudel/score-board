import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {RatingHistory} from '../../../models/ratingHistory';
import {RatingService} from '../../../services/rest/rating.service';
import {ThemeService} from '../../../services/common/theme.service';
import {ChartHelperService} from '../../../services/common/chart-helper.service';
import {Constants} from '../../../constants/constants';

export class PlayerData {
  playerName: string;
  dateTime: Date;
  value: number;
}

@Component({
  selector: 'app-rating-timeline',
  templateUrl: './rating-timeline.component.html',
  styleUrls: ['./rating-timeline.component.scss']
})
export class RatingTimelineComponent implements OnInit, OnDestroy {

  constructor(private ratingService: RatingService,
              private themeService: ThemeService) {
  }

  @Input()
  set gameId(val: string) {
    this.gameIdInternal = val;
    if (!this.gameIdInternal) {
      return;
    }
    this.createDataSource();
  }
  subscriptions = [];
  ratingHistories: RatingHistory[];
  gameIdInternal: string;
  data = [];
  columns = [];
  isDarkTheme = false;
  fontColor = '#000000';
  lineColor = 'lightgray';
  options;
  type = 'LineChart';

  ngOnInit() {
    this.subscriptions.push(this.ratingService.getAllRatingHistories().subscribe(
      r => {
        this.ratingHistories = r;
        this.createDataSource();
      }
    ));
    this.subscriptions.push(this.themeService.isDarkTheme.subscribe(x => {
      this.isDarkTheme  = x;
      if (x) {
        this.fontColor = '#ffffff';
        this.lineColor = '#575757';
      } else {
        this.fontColor = '#000000';
        this.lineColor = 'lightgray';
      }
      this.options = ChartHelperService.initChartOptions($localize`:@@conservativeRatingChartAxisTitle:Conservative Rating`,
        $localize`:@@timelineChartAxisTitle:Timeline`, this.fontColor, this.lineColor);
      this.options.hAxis.format = Constants.DateFormat;
    }));
  }

  private createDataSource() {
    this.data = [];
    this.columns = [];
    if (!this.ratingHistories || !this.gameIdInternal) {
      return;
    }
    this.columns = ['Timeline'];
    const playerMap: Map<string, number> = new Map<string, number>();
    const timelines = [];
    const playerDataList: PlayerData[] = [];
    let index = 1;
    for (const ratingHistory of this.ratingHistories) {
      if (ratingHistory.gameId === this.gameIdInternal) {
        const playerData = new PlayerData();
        playerData.playerName = ratingHistory.playerName;
        playerData.dateTime = ratingHistory.dateTime;
        playerData.value = ratingHistory.conservativeRating;
        playerDataList.push(playerData);
        if (this.columns.indexOf(ratingHistory.playerName) === -1) {
          this.columns.push(playerData.playerName);
          playerMap.set(playerData.playerName, index);
          index++;
        }
        if (timelines.indexOf(playerData.dateTime) === -1) {
          timelines.push(playerData.dateTime);
        }
      }
    }
    for (const timeline of timelines) {
      const dataRow = new Array<any>(this.columns.length);
      dataRow[0] = timeline;
      const currentValues = playerDataList.filter(p => p.dateTime === timeline);
      for (const value of currentValues) {
        const i = playerMap.get(value.playerName);
        dataRow[i] = value.value;
      }
      for (let i = 0; i < dataRow.length; i++) {
        if (!dataRow[i]) {
          if (this.data.length === 0) {
            dataRow[i] = 0;
          } else {
            dataRow[i] = this.data[this.data.length - 1][i];
          }
        }
      }
      this.data.push(dataRow);
    }
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
