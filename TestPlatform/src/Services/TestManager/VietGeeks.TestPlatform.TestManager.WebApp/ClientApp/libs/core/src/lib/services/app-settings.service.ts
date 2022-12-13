import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, of, switchMap } from 'rxjs';

export type IAppSettings = {
    appTitle?: string
}

@Injectable()
export class AppSettingsService {
  get appSettings(){
    return this._appSettings$.value;
  }

  private _appSettings$: BehaviorSubject<IAppSettings> = new BehaviorSubject<IAppSettings>({});

  constructor(private backend: HttpBackend) {

  }

  load<T extends IAppSettings>() {
    return (new HttpClient(this.backend)).get<T>('/Configuration').pipe(switchMap(rs => { this._appSettings$.next(rs); return of(true) }));
  }

  set<T extends IAppSettings>(value: T) {
    this._appSettings$.next(value);
  } 
}
 