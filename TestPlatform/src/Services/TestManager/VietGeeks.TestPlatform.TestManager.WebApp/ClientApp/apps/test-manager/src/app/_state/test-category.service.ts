import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { firstValueFrom, of } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { AppSettings } from '../app-setting.model';
import { TestCategory } from './test-category.model';
import { TestCategoryQuery } from './test-category.query';
import { TestCategoryStore } from './test-category.store';

@Injectable({ providedIn: 'root' })
export class TestCategoryService {
  constructor(
    private _testCategoriesQuery: TestCategoryQuery,
    private _testCategoriesStore: TestCategoryStore,
    private _appSettingService: AppSettingsService,
    private _http: HttpClient) {
  }

  get() {
    return firstValueFrom(this._testCategoriesQuery.selectHasCache().pipe(switchMap(hasCache => {
      const apiCall = this._http.get<TestCategory[]>(`${this.testManagerApiBaseUrl}/Management/TestCategory`).pipe(tap(entities => {
        this._testCategoriesStore.set(entities);
      }));

      return hasCache ? this._testCategoriesQuery.selectAll() : apiCall;
    })));
  }

  add(testCategory: Partial<TestCategory>) {
    return firstValueFrom(this._http.post<TestCategory>(`${this.testManagerApiBaseUrl}/Management/TestCategory`, testCategory)
      .pipe(tap(rs => {
        this._testCategoriesStore.add(rs);
      })));
  }

  update(id: string, testCategory: Partial<TestCategory>) {
    this._testCategoriesStore.update(id, testCategory);
  }

  remove(id: string) {
    return firstValueFrom(this._http.delete(`${this.testManagerApiBaseUrl}/Management/TestCategory`, { params: { ids: [id] } })
      .pipe(tap(() => this._testCategoriesStore.remove(id))));
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
