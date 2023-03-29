import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../app-setting.model';
import { EMPTY, firstValueFrom, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { PrivateAccessCodeType, Test, TestAccessType, TestInvitationStats, TestOverview } from './test.model';
import { TestsQuery } from './tests.query';
import { TestsStore } from './tests.store';
import { forEach, range } from 'lodash-es';
import { defKSUID32 } from '@thi.ng/ksuid';

@Injectable({ providedIn: 'root' })
export class TestsService {

  constructor(private _testsStore: TestsStore, private _testsQuery: TestsQuery, private _http: HttpClient, private _appSettingService: AppSettingsService) {
  }

  getOverviews() {
    return this._http.get<TestOverview[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition`);
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

  generateAccessCodes(count: number) {
    const codes: string[] = [];
    // https://en.wikipedia.org/wiki/Hexspeak
    const id = defKSUID32();
    forEach(range(count), () => {
      codes.push(id.next());
    });

    return codes;
  }

  generateRandomCode() {
    return defKSUID32().next();
  }

  getTestInvitationStats(id: string) {
    const test = this._testsQuery.getEntity(id);
    if (test === undefined || test.currentTestRun === null || (test.testAccessSettings.settings === null || test.testAccessSettings.accessType !== TestAccessType.PrivateAccessCode)) {
      return firstValueFrom(of([]));
    }
    const settings = test.testAccessSettings.settings as PrivateAccessCodeType;

    const model = {
      testRunId: test.currentTestRun.id,
      accessCodes: settings.configs.map(c => c.code)
    };

    return firstValueFrom(this._http.post<TestInvitationStats[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/TestInvitationStats`, model));
  }

  activate(id: string) {
    return firstValueFrom(this._http.post(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/Activate`, null).pipe(tap(rs => {
      this._testsStore.update(id, rs);
    })));
  }

  end(id: string) {
    return firstValueFrom(this._http.post(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/End`, null).pipe(tap(rs => {
      this._testsStore.update(id, rs);
    })));
  }

  restart(id: string) {
    return firstValueFrom(this._http.post(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/Restart`, null).pipe(tap(rs => {
      this._testsStore.update(id, rs);
    })));
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
