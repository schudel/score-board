import { TestBed } from '@angular/core/testing';

import { LiveMatchHelperService } from './live-match-helper.service';

describe('LiveMatchHelperService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LiveMatchHelperService = TestBed.get(LiveMatchHelperService);
    expect(service).toBeTruthy();
  });
});
