import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchesStatisticsComponent } from './matches-statistics.component';

describe('MatchesStatisticsComponent', () => {
  let component: MatchesStatisticsComponent;
  let fixture: ComponentFixture<MatchesStatisticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MatchesStatisticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MatchesStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
