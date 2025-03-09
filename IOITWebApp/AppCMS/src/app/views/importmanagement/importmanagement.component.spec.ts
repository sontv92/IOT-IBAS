import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportManagement } from './importmanagement.component';

describe('importmanagement', () => {
  let component: ImportManagement;
  let fixture: ComponentFixture<ImportManagement>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ImportManagement ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
