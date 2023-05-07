import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { PagedSearchRequest, PagedSearchResponse } from '@viet-geeks/shared';
import { firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AppSettings } from '../../../app-setting.model';
import { Question, QuestionSummary } from './question.model';
import { QuestionStore } from './question.store';

@Injectable({ providedIn: 'root' })
export class QuestionService {
  constructor(private _questionStore: QuestionStore, private _http: HttpClient, private _appSettingService: AppSettingsService) {
  }

  get(testId: string, pagedSearchParams: PagedSearchRequest) {
    return this._http.get<PagedSearchResponse<Question>>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question`, {
      params: Object.assign({}, pagedSearchParams)
    }).pipe(tap(rs => this._questionStore.set(rs.results)));
  }

  getOrders(testId: string) {
    return this._http.get<Question[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/Order`);
  }

  updateOrders(testId: string, orders: any[]) {
    return this._http.put<Question[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/Order`, orders);
  }

  getQuestion(testId: string, questionId: string) {
    return this._http.get<Question>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/${questionId}`).pipe(tap(rs => {
      this._questionStore.upsert(questionId, rs);
    }));
  }

  add(testId: string, question: Question) {
    return this._http.post<Question>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question`, question).pipe(tap(rs => {
      this._questionStore.add(rs);
    }));
  }

  update(testId: string, questionId: string, question: Partial<Question>) {
    return firstValueFrom(this._http.put<Question>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question/${questionId}`, question).pipe(tap(rs => {
      this._questionStore.update(questionId, rs);
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

