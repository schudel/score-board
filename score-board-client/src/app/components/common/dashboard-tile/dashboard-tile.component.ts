import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html',
  styleUrls: ['./dashboard-tile.component.scss']
})
export class DashboardTileComponent implements OnInit {

  constructor() { }

  @Input() link: any;
  @Input() label: string;

  ngOnInit() {
  }
}
