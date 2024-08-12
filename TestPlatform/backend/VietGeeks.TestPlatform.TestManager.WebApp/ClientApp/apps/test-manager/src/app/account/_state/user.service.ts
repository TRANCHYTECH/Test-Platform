import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { User } from './user.model';
import { UserStore } from './user.store';
import { firstValueFrom, of } from 'rxjs';
import { UserQuery } from './user.query';
import { AppSettingsService } from '@viet-geeks/core';
import { AppSettings } from '../../app-setting.model';

@Injectable({ providedIn: 'root' })
export class UserService {

  private _cachedTimeZones: string[] = [];

  constructor(private userStore: UserStore, private userQuery: UserQuery, private _appSettingService: AppSettingsService, private http: HttpClient) {
  }

  get() {
    return this.http.get<User>(`${this.accountManagerApiBaseUrl}/Account/User`).pipe(tap(entity => {
      this.userStore.set([entity]);
    }));
  }

  add(user: User) {
    this.userStore.add(user);
  }

  update(id: string, user: Partial<User>) {
    return firstValueFrom(this.http.put<User>(`${this.accountManagerApiBaseUrl}/Account/User`, user).pipe(tap(entity => {
      this.userStore.update(id, entity);
    })));
  }

  getTimeZones() {
    if (this._cachedTimeZones.length > 0) {
      return of(this._cachedTimeZones);
    }

    return this.http.get<string[]>(`${this.accountManagerApiBaseUrl}/Account/TimeZones`).pipe(tap(timeZones => {
      this._cachedTimeZones = timeZones;
    }));
  }

  private get accountManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().accountManagerApiBaseUrl;
  }
}
