import { TestBed } from '@angular/core/testing';

import { RatingService } from './rating.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('RatingService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [RatingService]
  }));

  it('should be created', () => {
    const service: RatingService = TestBed.get(RatingService);
    expect(service).toBeTruthy();
  });
});
