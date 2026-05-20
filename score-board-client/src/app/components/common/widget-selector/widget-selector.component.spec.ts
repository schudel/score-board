import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetSelectorComponent } from './widget-selector.component';

describe('WidgetSelectorComponent', () => {
  let component: WidgetSelectorComponent;
  let fixture: ComponentFixture<WidgetSelectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
