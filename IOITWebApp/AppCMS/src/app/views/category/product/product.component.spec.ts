import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CateProductComponent } from './product.component';

describe('ProductComponent', () => {
  let component: CateProductComponent;
  let fixture: ComponentFixture<CateProductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CateProductComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CateProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
