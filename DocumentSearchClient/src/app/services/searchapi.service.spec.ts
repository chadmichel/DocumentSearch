import { TestBed } from '@angular/core/testing';

import { SearchapiService } from './searchapi.service';

describe('SearchapiService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SearchapiService = TestBed.get(SearchapiService);
    expect(service).toBeTruthy();
  });
});
