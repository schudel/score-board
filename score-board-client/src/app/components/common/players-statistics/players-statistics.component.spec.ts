import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayersStatisticsComponent } from './players-statistics.component';

describe('PlayersStatisticsComponent', () => {
  let component: PlayersStatisticsComponent;
  let fixture: ComponentFixture<PlayersStatisticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlayersStatisticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlayersStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
