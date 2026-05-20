import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoreBoardInfoComponent } from './score-board-info.component';

describe('ScoreBoardInfoComponent', () => {
  let component: ScoreBoardInfoComponent;
  let fixture: ComponentFixture<ScoreBoardInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScoreBoardInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScoreBoardInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
