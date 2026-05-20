import { TestBed } from '@angular/core/testing';

import { BaseRestService } from './base-rest.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('BaseRestService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [BaseRestService]
  }));

  it('should be created', () => {
    const service: BaseRestService = TestBed.get(BaseRestService);
    expect(service).toBeTruthy();
  });
});
