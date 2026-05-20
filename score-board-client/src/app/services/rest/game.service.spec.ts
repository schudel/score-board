import { TestBed } from '@angular/core/testing';

import { GameService } from './game.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('GameService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [GameService]
  }));

  it('should be created', () => {
    const service: GameService = TestBed.get(GameService);
    expect(service).toBeTruthy();
  });
});
