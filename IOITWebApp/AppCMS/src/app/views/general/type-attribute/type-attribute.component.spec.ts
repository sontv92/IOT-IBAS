import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TypeAttributeComponent } from './type-attribute.component';

describe('TypeAttributeComponent', () => {
  let component: TypeAttributeComponent;
  let fixture: ComponentFixture<TypeAttributeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TypeAttributeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TypeAttributeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
