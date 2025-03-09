import { TestBed } from '@angular/core/testing';

import { CallCategoryFunctionService } from './call-category-function.service';

describe('CallCategoryFunctionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CallCategoryFunctionService = TestBed.get(CallCategoryFunctionService);
    expect(service).toBeTruthy();
  });
});
