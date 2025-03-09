import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AttribuiteComponent } from './attribuite.component';

describe('AttribuiteComponent', () => {
  let component: AttribuiteComponent;
  let fixture: ComponentFixture<AttribuiteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AttribuiteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AttribuiteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
