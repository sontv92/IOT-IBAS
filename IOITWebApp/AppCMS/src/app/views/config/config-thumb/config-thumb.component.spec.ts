import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigThumbComponent } from './config-thumb.component';

describe('ConfigThumbComponent', () => {
  let component: ConfigThumbComponent;
  let fixture: ComponentFixture<ConfigThumbComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigThumbComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigThumbComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
