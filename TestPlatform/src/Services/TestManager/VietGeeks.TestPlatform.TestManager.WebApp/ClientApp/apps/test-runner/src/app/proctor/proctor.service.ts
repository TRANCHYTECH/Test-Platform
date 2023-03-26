import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { catchError, Observable, of } from 'rxjs';
import { AppSettings } from '../app-setting.model';
import { FinishedExam } from '../state/exam-content.model';
import { ApiExamService } from '../api/services';
import { StartExamOutput } from '../api/models';

@Injectable({
  providedIn: 'root'
})
export class ProctorService {
  private _appSettingService = inject(AppSettingsService);
  private _examService = inject(ApiExamService);

  private _httpClient = inject(HttpClient);

  get testRunnerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testRunnerApiBaseUrl;
  }

  verifyTest(input: Partial<{ accessCode: string, testId: string }>) {
    return this._examService.verify({
      body: input
    }).pipe(catchError(error => {
      console.log('error', error);
      return of(null);
    }));
  }

  provideExamineeInfo(examineeInfo: { [key: string]: string }) {
    return this._examService.provideExamineeInfo({ body: examineeInfo });
  }

  startExam(): Observable<StartExamOutput> {
    return this._examService.startExam();
  }

  submitAnswer(answer: { questionId: string, answerIds: string[] }) {
    return this._examService.submitAnswer({body: answer}).pipe(catchError(error => {
      console.log('submit answer error', error);
      return of(null);
    }));
  }

  finishExam() {
    return this._httpClient.post<FinishedExam>(`${this.testRunnerApiBaseUrl}/Exam/Finish`, null);
  }
}
