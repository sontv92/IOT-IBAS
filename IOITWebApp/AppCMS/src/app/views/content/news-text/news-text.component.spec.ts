import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewsTextComponent } from './news-text.component';

describe('NewsTextComponent', () => {
  let component: NewsTextComponent;
  let fixture: ComponentFixture<NewsTextComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewsTextComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewsTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
