import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { switchMap, tap } from 'rxjs/operators';
import { TestCategory } from './test-category.model';
import { TestCategoriesStore } from './test-categories.store';
import { TestCategoriesQuery } from './test-categories.query';
import { of } from 'rxjs';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../app-setting.model';

@Injectable({ providedIn: 'root' })
export class TestCategoriesService {

  constructor(
    private _testCategoriesQuery: TestCategoriesQuery,
    private _testCategoriesStore: TestCategoriesStore,
    private _appSettingService: AppSettingsService,
    private _http: HttpClient) {
  }

  get() {
    return this._testCategoriesQuery.selectHasCache().pipe(switchMap(hasCache => {
      const apiCall = this._http.get<TestCategory[]>(`${this.testManagerApiBaseUrl}/Management/TestCategory`).pipe(tap(entities => {
      this._testCategoriesStore.set(entities);
      }));

      return hasCache ? of([]) : apiCall;
    }))
  }

  add(testCategory: Partial<TestCategory>) {
    return this._http.post<TestCategory>(`${this.testManagerApiBaseUrl}/Management/TestCategory`, testCategory).pipe(tap(rs => {
      this._testCategoriesStore.add(rs);
    }));
  }

  update(id: string, testCategory: Partial<TestCategory>) {
    this._testCategoriesStore.update(id, testCategory);
  }

  remove(id: ID) {
    this._testCategoriesStore.remove(id);
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
