import { TestBed } from '@angular/core/testing';

import { GameHelperService } from './game-helper.service';

describe('GameHelperService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GameHelperService = TestBed.get(GameHelperService);
    expect(service).toBeTruthy();
  });
});
