import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ID } from '@datorama/akita';
import { tap } from 'rxjs/operators';
import { ExamSummary, TestRunSummary } from './exam-summary.model';
import { ExamSummaryStore } from './exam-summary.store';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../app-setting.model';

@Injectable({ providedIn: 'root' })
export class ExamSummaryService {

  private _appSettingService = inject(AppSettingsService);

  constructor(private examSummaryStore: ExamSummaryStore, private http: HttpClient) {
  }

  getTestRuns(testId: string) {
    return this.http.get<TestRunSummary[]>(`${this.testManagerApiBaseUrl}/Management/TestReport/${testId}/TestRuns`);
  }

  get(testRunIds: string[]) {
    return this.http.get<ExamSummary[]>(`${this.testManagerApiBaseUrl}/Management/TestReport/Exams`, { params: { 'testRunIds': testRunIds } }).pipe(tap(entities => {
      this.examSummaryStore.set(entities);
    }));
  }

  add(examSummary: ExamSummary) {
    this.examSummaryStore.add(examSummary);
  }

  update(id: string, examSummary: Partial<ExamSummary>) {
    this.examSummaryStore.update(id, examSummary);
  }

  remove(id: ID) {
    this.examSummaryStore.remove(id);
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }

}
