import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../app-setting.model';
import { EMPTY, firstValueFrom } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Test } from './test.model';
import { TestsQuery } from './tests.query';
import { TestsStore } from './tests.store';

@Injectable({ providedIn: 'root' })
export class TestsService {

  constructor(private _testsStore: TestsStore, private _testsQuery: TestsQuery, private _http: HttpClient, private _appSettingService: AppSettingsService) {
  }


  get() {
    return this._http.get<Test[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition`).pipe(tap(entities => {
      this._testsStore.set(entities);
    }));
  }

  getById(id: string) {
    if (this._testsQuery.hasEntity(id)) {
      return EMPTY;
    }

    return this._http.get<Test>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}`).pipe(
      tap(rs => {
        this._testsStore.upsert(id, rs);
      }),
      catchError((err => {
        console.log('http client error', err);
        return EMPTY;
      })));
  }

  setActive(id: string) {
    this._testsStore.setActive(id);
  }

  add(test: Partial<Test>) {
    return this._http.post<Test>(`${this.testManagerApiBaseUrl}/Management/TestDefinition`, test).pipe(tap(rs => {
      this._testsStore.add(rs);
    }));
  }

  update(id: string, test: Partial<Test>) {
    return firstValueFrom(this._http.put<Test>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}`, test).pipe(tap(rs => {
      this._testsStore.update(id, rs);
    })));
  }

  remove(id: string) {
    this._testsStore.remove(id);
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
