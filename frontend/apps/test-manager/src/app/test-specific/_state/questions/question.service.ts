import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  PagedSearchRequest,
  PagedSearchResponse,
  Question,
  QuestionSummary,
} from '@viet-geeks/shared';
import { firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import { QuestionStore } from './question.store';

@Injectable({ providedIn: 'root' })
export class QuestionService {
  constructor(
    private _questionStore: QuestionStore,
    private _http: HttpClient
  ) {}

  get(testId: string, pagedSearchParams: PagedSearchRequest) {
    return this._http
      .get<PagedSearchResponse<Question>>(
        `/TestManager:/Management/TestDefinition/${testId}/Question`,
        {
          params: Object.assign({}, pagedSearchParams),
        }
      )
      .pipe(tap((rs) => this._questionStore.set(rs.results)));
  }

  getOrders(testId: string) {
    return this._http.get<Question[]>(
      `/TestManager:/Management/TestDefinition/${testId}/Question/Order`
    );
  }

  updateOrders(testId: string, orders: any[]) {
    return this._http.put<Question[]>(
      `/TestManager:/Management/TestDefinition/${testId}/Question/Order`,
      orders
    );
  }

  getQuestion(testId: string, questionId: string) {
    return this._http
      .get<Question>(
        `/TestManager:/Management/TestDefinition/${testId}/Question/${questionId}`
      )
      .pipe(
        tap((rs) => {
          this._questionStore.upsert(questionId, rs);
        })
      );
  }

  add(testId: string, question: Question) {
    return this._http
      .post<Question>(
        `/TestManager:/Management/TestDefinition/${testId}/Question`,
        question
      )
      .pipe(
        tap((rs) => {
          this._questionStore.add(rs);
        })
      );
  }

  update(testId: string, questionId: string, question: Partial<Question>) {
    return firstValueFrom(
      this._http
        .put<Question>(
          `/TestManager:/Management/TestDefinition/${testId}/Question/${questionId}`,
          question
        )
        .pipe(
          tap((rs) => {
            this._questionStore.update(questionId, rs);
          })
        )
    );
  }

  remove(testId: string, id: string) {
    return firstValueFrom(
      this._http
        .delete(
          `/TestManager:/Management/TestDefinition/${testId}/Question/${id}`
        )
        .pipe(
          tap(() => {
            this._questionStore.remove(id);
          })
        )
    );
  }

  getSummary(testId: string) {
    return firstValueFrom(
      this._http.get<QuestionSummary[]>(
        `/TestManager:/Management/TestDefinition/${testId}/Question/Summary`
      ),
      { defaultValue: [] }
    );
  }
}
