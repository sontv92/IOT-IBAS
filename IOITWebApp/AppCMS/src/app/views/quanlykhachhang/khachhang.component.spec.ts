import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KhachHangComponent } from './khachhang.component';

describe('KhachHangComponent', () => {
  let component: KhachHangComponent;
  let fixture: ComponentFixture<KhachHangComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KhachHangComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KhachHangComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
