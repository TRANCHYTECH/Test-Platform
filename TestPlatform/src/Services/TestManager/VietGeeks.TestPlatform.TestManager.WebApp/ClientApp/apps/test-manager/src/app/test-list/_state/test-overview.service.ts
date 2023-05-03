import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { AppSettingsService } from "@viet-geeks/core";
import { firstValueFrom } from "rxjs";
import { AppSettings } from "../../app-setting.model";
import { TestOverview } from "./test-overview.model";

@Injectable({ providedIn: 'root' })
export class TestOverviewService {
    private _http = inject(HttpClient);
    private _appSettingService = inject(AppSettingsService);

    get() {
        return firstValueFrom(this._http.get<TestOverview[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition`));
    }

    private get testManagerApiBaseUrl() {
        return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
    }
}
