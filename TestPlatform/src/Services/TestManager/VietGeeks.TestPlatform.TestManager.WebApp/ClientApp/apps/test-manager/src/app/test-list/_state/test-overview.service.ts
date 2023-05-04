import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { AppSettingsService } from "@viet-geeks/core";
import { AppSettings } from "../../app-setting.model";
import { TestOverview } from "./test-overview.model";
import { PagedSearchRequest, PagedSearchResponse } from "@viet-geeks/shared";

@Injectable({ providedIn: 'root' })
export class TestOverviewService {
    private _http = inject(HttpClient);
    private _appSettingService = inject(AppSettingsService);

    get(pagedSearchParams: PagedSearchRequest) {
        return this._http.get<PagedSearchResponse<TestOverview>>(`${this.testManagerApiBaseUrl}/Management/TestDefinition`, {
            params: Object.assign({}, pagedSearchParams)
        });
    }

    private get testManagerApiBaseUrl() {
        return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
    }
}
