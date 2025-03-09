import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OlCategoryComponent } from './ol-category.component';

describe('OlCategoryComponent', () => {
  let component: OlCategoryComponent;
  let fixture: ComponentFixture<OlCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OlCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OlCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
