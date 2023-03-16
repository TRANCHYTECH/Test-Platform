import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../app-setting.model';
import { ExamContent } from '../state/exam-content.model';

@Injectable({
  providedIn: 'root'
})
export class ProctorService {

  private _appSettingService = inject(AppSettingsService);

  private _httpClient = inject(HttpClient);

  get testRunnerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testRunnerApiBaseUrl;
  }

  verifyTest(accessCode: string) {
    return this._httpClient.post(`${this.testRunnerApiBaseUrl}/Exam/PreStart/Verify`, { accessCode: accessCode });
  }

  provideExamineeInfo(examineeInfo: { [key: string]: string }) {
    return this._httpClient.post(`${this.testRunnerApiBaseUrl}/Exam/PreStart/ProvideExamineeInfo`, examineeInfo);
  }

  startExam() {
    return this._httpClient.post<ExamContent>(`${this.testRunnerApiBaseUrl}/Exam/Start`, null);
  }
}
