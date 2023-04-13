import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ExamSummary, TestRunSummary } from './exam-summary.model';
import { ExamSummaryStore } from './exam-summary.store';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../../app-setting.model';

@Injectable({ providedIn: 'root' })
export class ExamSummaryService {

  private _appSettingService = inject(AppSettingsService);

  constructor(private examSummaryStore: ExamSummaryStore, private http: HttpClient) {
  }

  getTestRuns(testId: string) {
    return this.http.get<TestRunSummary[]>(`${this.testManagerApiBaseUrl}/Management/TestReport/${testId}/TestRuns`);
  }

  get(testRunIds: string[]) {
    return this.http.get<ExamSummary[]>(`${this.testManagerApiBaseUrl}/Management/TestReport/Exams`, { params: { 'testRunIds': testRunIds } });
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}