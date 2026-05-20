import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NumberOneWidgetComponent } from './number-one-widget.component';

describe('NumberOneWidgetComponent', () => {
  let component: NumberOneWidgetComponent;
  let fixture: ComponentFixture<NumberOneWidgetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NumberOneWidgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NumberOneWidgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
