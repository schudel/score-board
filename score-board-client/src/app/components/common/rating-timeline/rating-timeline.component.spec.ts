import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RatingTimelineComponent } from './rating-timeline.component';

describe('RatingTimelineComponent', () => {
  let component: RatingTimelineComponent;
  let fixture: ComponentFixture<RatingTimelineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RatingTimelineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RatingTimelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
