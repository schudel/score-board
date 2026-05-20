import { TestBed } from '@angular/core/testing';

import { TeamService } from './team.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('TeamService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [TeamService]
  }));

  it('should be created', () => {
    const service: TeamService = TestBed.get(TeamService);
    expect(service).toBeTruthy();
  });
});
