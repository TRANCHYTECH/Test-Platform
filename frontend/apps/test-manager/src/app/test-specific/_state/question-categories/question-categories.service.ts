import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { tap } from 'rxjs/operators';
import { QuestionCategory } from './question-categories.model';
import { QuestionCategoriesStore } from './question-categories.store';
import { firstValueFrom } from 'rxjs';
import { QuestionCategoriesQuery } from './question-categories.query';

@Injectable({ providedIn: 'root' })
export class QuestionCategoriesService {
  private _questionCategoriesStore = inject(QuestionCategoriesStore);
  private _questionCategoryQuery = inject(QuestionCategoriesQuery);

  private _http = inject(HttpClient);

  //todo: how to cache by test id.
  get(testId: string) {
    return this._http
      .get<QuestionCategory[]>(
        `/TestManager:/Management/TestDefinition/${testId}/QuestionCategory`
      )
      .pipe(
        tap((entities) => {
          this._questionCategoriesStore.set(entities);
        })
      );
  }

  add(testId: string, questionCategory: Partial<QuestionCategory>) {
    return this._http
      .post<QuestionCategory>(
        `/TestManager:/Management/TestDefinition/${testId}/QuestionCategory`,
        questionCategory
      )
      .pipe(
        tap((rs) => {
          this._questionCategoriesStore.add(rs);
        })
      );
  }

  update(testId: string, questionCategory: Partial<QuestionCategory>) {
    this._questionCategoriesStore.update(testId, questionCategory);
  }

  remove(testId: string, categoryId: string) {
    return firstValueFrom(
      this._http
        .delete(
          `/TestManager:/Management/TestDefinition/${testId}/QuestionCategory`,
          { params: { ids: [categoryId] } }
        )
        .pipe(
          tap(() => {
            this._questionCategoriesStore.remove(categoryId);
          })
        )
    );
  }
}
