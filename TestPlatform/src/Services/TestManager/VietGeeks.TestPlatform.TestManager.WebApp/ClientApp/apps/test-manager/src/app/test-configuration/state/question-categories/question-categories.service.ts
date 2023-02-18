import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { AppSettingsService } from '@viet-geeks/core';
import { tap } from 'rxjs/operators';
import { AppSettings } from '../../../app-setting.model';
import { QuestionCategory } from './question-categories.model';
import { QuestionCategoriesStore } from './question-categories.store';

@Injectable({ providedIn: 'root' })
export class QuestionCategoriesService {

  constructor(private _questionCategoriesStore: QuestionCategoriesStore, private _http: HttpClient, private _appSettingService: AppSettingsService) {
  }

  get() {
    return this._http.get<QuestionCategory[]>(`${this.testManagerApiBaseUrl}/Management/QuestionCategory`).pipe(tap(entities => {
      this._questionCategoriesStore.set(entities);
    }));
  }

  add(testCategory: Partial<QuestionCategory>) {
    return this._http.post<QuestionCategory>(`${this.testManagerApiBaseUrl}/Management/QuestionCategory`, testCategory).pipe(tap(rs => {
      this._questionCategoriesStore.add(rs);
    }));
  }

  update(id: string, questionCategory: Partial<QuestionCategory>) {
    this._questionCategoriesStore.update(id, questionCategory);
  }

  remove(id: ID) {
    this._questionCategoriesStore.remove(id);
  }


  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
