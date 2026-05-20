import { TestBed } from '@angular/core/testing';

import { RatingHelperService } from './rating-helper.service';

describe('RatingHelperService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RatingHelperService = TestBed.get(RatingHelperService);
    expect(service).toBeTruthy();
  });
});
