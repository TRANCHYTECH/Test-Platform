import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { TestCategory } from './test-category.model';
import { TestCategoryQuery } from './test-category.query';
import { TestCategoryStore } from './test-category.store';

@Injectable({ providedIn: 'root' })
export class TestCategoryService {
  constructor(
    private _testCategoriesQuery: TestCategoryQuery,
    private _testCategoriesStore: TestCategoryStore,
    private _http: HttpClient
  ) {}

  get() {
    return firstValueFrom(
      this._testCategoriesQuery.selectHasCache().pipe(
        switchMap((hasCache) => {
          const apiCall = this._http
            .get<TestCategory[]>(`/TestManager:/Management/TestCategory`)
            .pipe(
              tap((entities) => {
                this._testCategoriesStore.set(entities);
              })
            );

          return hasCache ? this._testCategoriesQuery.selectAll() : apiCall;
        })
      )
    );
  }

  add(testCategory: Partial<TestCategory>) {
    return firstValueFrom(
      this._http
        .post<TestCategory>(
          `/TestManager:/Management/TestCategory`,
          testCategory
        )
        .pipe(
          tap((rs) => {
            this._testCategoriesStore.add(rs);
          })
        )
    );
  }

  update(id: string, testCategory: Partial<TestCategory>) {
    this._testCategoriesStore.update(id, testCategory);
  }

  remove(id: string) {
    return firstValueFrom(
      this._http
        .delete(`/TestManager:/Management/TestCategory`, {
          params: { ids: [id] },
        })
        .pipe(tap(() => this._testCategoriesStore.remove(id)))
    );
  }
}
