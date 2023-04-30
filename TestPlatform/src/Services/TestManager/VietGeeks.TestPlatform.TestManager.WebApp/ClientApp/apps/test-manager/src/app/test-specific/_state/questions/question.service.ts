import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AppSettings } from '../../../app-setting.model';
import { Question, QuestionSummary } from './question.model';
import { QuestionStore } from './question.store';

@Injectable({ providedIn: 'root' })
export class QuestionService {

  constructor(private _questionStore: QuestionStore, private _http: HttpClient, private _appSettingService: AppSettingsService) {
  }

  get(testId: string) {
    return this._http.get<Question[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question`).pipe(tap(entities => {
      this._questionStore.set(entities);
    }));
  }

  add(testId: string, question: Question) {
    return this._http.post<Question>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question`, question).pipe(tap(qs => {
      this._questionStore.add(qs);
    }));
  }

  update(testId: string, id: string, question: Partial<Question>) {
    return firstValueFrom(this._http.put<Question>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/${id}`, question).pipe(tap(rs => {
      this._questionStore.update(id, rs);
    })));
  }

  remove(testId: string, id: string) {
    return firstValueFrom(this._http.delete(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/${id}`).pipe(tap(() => {
      this._questionStore.remove(id);
    })));
  }

  getSummary(testId: string) {
    return firstValueFrom(this._http.get<QuestionSummary[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/Summary`), { defaultValue: [] });
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
