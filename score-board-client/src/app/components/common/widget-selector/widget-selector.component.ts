import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Player} from '../../../models/player';
import {Match} from '../../../models/match';
import {DashboardWidget} from '../../../models/dashboard-widget';
import {Rating} from '../../../models/rating';

@Component({
  selector: 'app-widget-selector',
  templateUrl: './widget-selector.component.html',
  styleUrls: ['./widget-selector.component.scss']
})
export class WidgetSelectorComponent implements OnInit {
  widgetValue = '';
  widgets: Array<DashboardWidget>;

  @Input() get widget() {
    return this.widgetValue;
  }
  set widget(val) {
    this.widgetValue = val;
    this.widgetChange.emit(this.widgetValue);
  }
  @Input() players: Player[];
  @Input() currentPlayer: Player;
  @Input() recentMatches: Match[];
  @Input() epicMatches: Match[];
  @Input() myMatches: Match[];
  @Input() ratings: Rating[];
  @Input() ranking: number;
  @Input() highestScore: number;

  @Output() removeButtonClicked = new EventEmitter();
  @Output() widgetChange = new EventEmitter();

  constructor() {
    this.widgets = DashboardWidget.getWidgets();
  }

  ngOnInit() {
  }

  removeWidget() {
    this.removeButtonClicked.emit();
  }

  getTranslatedWidgetName(displayName: string) {
    // TODO: fix localization
    return displayName;
    // return $localize`:@@${displayName}:${displayName}`;
  }
}
