import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RankingWidgetComponent } from './ranking-widget.component';

describe('RankingWidgetComponent', () => {
  let component: RankingWidgetComponent;
  let fixture: ComponentFixture<RankingWidgetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RankingWidgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RankingWidgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
