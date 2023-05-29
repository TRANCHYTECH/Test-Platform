import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../../app-setting.model';
import { EMPTY, firstValueFrom, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { PrivateAccessCodeType, Test, TestAccessType, TestInvitationStats } from './test.model';
import { TestsQuery } from './tests.query';
import { TestsStore } from './tests.store';
import { defKSUID32 } from '@thi.ng/ksuid';

@Injectable({ providedIn: 'root' })
export class TestsService {
  private _testsStore = inject(TestsStore);
  private _testsQuery = inject(TestsQuery);
  private _http = inject(HttpClient);
  private _appSettingService = inject(AppSettingsService);

  getById(id: string) {
    if (this._testsQuery.hasEntity(id)) {
      return EMPTY;
    }

    return this._http.get<Test>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}`).pipe(
      tap(rs => {
        this._testsStore.upsert(id, rs);
      }),
      catchError((() => {
        return EMPTY;
      })));
  }

  setActive(id: string) {
    this._testsStore.setActive(id);
  }

  removeCurrentActive() {
    const id = this._testsQuery.getActiveId();
    if (id !== null && id !== undefined) {
      this._testsStore.removeActive(id);
    }
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

  generateAccessCodes(id: string, quantity: number) {
    return firstValueFrom(this._http.get<{ accessCodes: string[] }>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/TestAccess/GenerateAccessCodes/${quantity}`).pipe(switchMap(rs => {
      return of(rs.accessCodes);
    })));
  }

  removeAccessCode(id: string, code: string) {
    return firstValueFrom(this._http.delete(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/TestAccess/RemoveAccessCode/${code}`));
  }

  removeAccessCodes(id: string, codes: string[]) {
    return firstValueFrom(this._http.delete(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/TestAccess/RemoveAccessCodes`, {
      params: { code: codes }
    }));
  }

  sendAccessCodes(id: string, codes: string[]) {
    return firstValueFrom(this._http.post(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${id}/TestAccess/SendAccessCodes`, codes));
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
