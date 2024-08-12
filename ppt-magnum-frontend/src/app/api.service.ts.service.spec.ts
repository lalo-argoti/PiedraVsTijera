import { TestBed } from '@angular/core/testing';

import { ApiServiceTsService } from './api.service.ts.service';

describe('ApiServiceTsService', () => {
  let service: ApiServiceTsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiServiceTsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
