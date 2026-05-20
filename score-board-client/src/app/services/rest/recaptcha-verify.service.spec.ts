import { TestBed } from '@angular/core/testing';

import { RecaptchaVerifyService } from './recaptcha-verify.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('RecaptchaVerifyService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [RecaptchaVerifyService]
  }));

  it('should be created', () => {
    const service: RecaptchaVerifyService = TestBed.get(RecaptchaVerifyService);
    expect(service).toBeTruthy();
  });
});
