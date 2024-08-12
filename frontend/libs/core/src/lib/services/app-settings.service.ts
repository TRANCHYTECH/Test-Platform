import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type IAppSettings = {
    appTitle?: string
}

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  get appSettings(){
    return this._appSettings$.value;
  }

  private _appSettings$: BehaviorSubject<IAppSettings> = new BehaviorSubject<IAppSettings>({});

  set<T extends IAppSettings>(value: T) {
    this._appSettings$.next(value);
  } 

  get<T extends IAppSettings>() {
    return <T>this._appSettings$.value;
  } 
}