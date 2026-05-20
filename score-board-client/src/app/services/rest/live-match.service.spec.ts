import { TestBed } from '@angular/core/testing';

import { LiveMatchService } from './live-match.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('LiveMatchService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [LiveMatchService]
  }));

  it('should be created', () => {
    const service: LiveMatchService = TestBed.get(LiveMatchService);
    expect(service).toBeTruthy();
  });
});
