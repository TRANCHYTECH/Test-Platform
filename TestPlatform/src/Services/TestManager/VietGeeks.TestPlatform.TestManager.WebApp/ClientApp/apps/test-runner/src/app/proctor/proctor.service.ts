import { inject, Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../app-setting.model';

@Injectable({
  providedIn: 'root'
})
export class ProctorService {

  private _appSettingService = inject(AppSettingsService);

  get testRunnerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testRunnerApiBaseUrl;
  }
}
