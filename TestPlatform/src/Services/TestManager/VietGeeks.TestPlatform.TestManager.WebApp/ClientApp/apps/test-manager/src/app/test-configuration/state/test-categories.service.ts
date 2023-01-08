import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { switchMap, tap } from 'rxjs/operators';
import { TestCategory } from './test-category.model';
import { TestCategoriesStore } from './test-categories.store';
import { TestCategoriesQuery } from './test-categories.query';
import { EMPTY } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TestCategoriesService {

  constructor(
    private _testCategoriesQuery: TestCategoriesQuery,
    private _testCategoriesStore: TestCategoriesStore,
    private _http: HttpClient) {
  }

  get() {
    return this._testCategoriesQuery.selectHasCache().pipe(switchMap(hasCache => {
      const apiCall = this._http.get<TestCategory[]>('https://63b42852ea89e3e3db580795.mockapi.io/api/testcategories').pipe(tap(entities => {
      this._testCategoriesStore.set(entities);
      }));

      return hasCache ? EMPTY : apiCall;
    }))
  }

  add(testCategory: TestCategory) {
    this._testCategoriesStore.add(testCategory);
  }

  update(id: string, testCategory: Partial<TestCategory>) {
    this._testCategoriesStore.update(id, testCategory);
  }

  remove(id: ID) {
    this._testCategoriesStore.remove(id);
  }
}
