import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GamesStatisticsComponent } from './games-statistics.component';

describe('GamesStatisticsComponent', () => {
  let component: GamesStatisticsComponent;
  let fixture: ComponentFixture<GamesStatisticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GamesStatisticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GamesStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
