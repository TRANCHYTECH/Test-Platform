import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { TestOverview } from './test-overview.model';
import { PagedSearchRequest, PagedSearchResponse } from '@viet-geeks/shared';

@Injectable({ providedIn: 'root' })
export class TestOverviewService {
  private _http = inject(HttpClient);

  get(pagedSearchParams: PagedSearchRequest) {
    return this._http.get<PagedSearchResponse<TestOverview>>(
      `/TestManager:/Management/TestDefinition`,
      {
        params: Object.assign({}, pagedSearchParams),
      }
    );
  }
}
