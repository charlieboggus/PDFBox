import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopbanneradComponent } from './topbannerad.component';

describe('TopbanneradComponent', () => {
  let component: TopbanneradComponent;
  let fixture: ComponentFixture<TopbanneradComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopbanneradComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopbanneradComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
