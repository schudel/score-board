import { TestBed } from '@angular/core/testing';

import { PlayerService } from './player.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('PlayerService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [PlayerService]
  }));

  it('should be created', () => {
    const service: PlayerService = TestBed.get(PlayerService);
    expect(service).toBeTruthy();
  });
});
