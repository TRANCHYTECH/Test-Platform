import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { AppSettingsService } from '@viet-geeks/core';
import { tap } from 'rxjs/operators';
import { AppSettings } from '../../../app-setting.model';
import { Question } from './question.model';
import { QuestionsStore } from './question.store';

@Injectable({ providedIn: 'root' })
export class QuestionService {

  constructor(private _questionStore: QuestionsStore, private http: HttpClient, private _appSettingService: AppSettingsService) {
  }

  get(testId: string) {
    return this.http.get<Question[]>(`${this.testManagerApiBaseUrl}/Management/TestDefinition/${testId}/Question`).pipe(tap(entities => {
      this._questionStore.set(entities);
    }));
  }

  add(testCategory: Question) {
    this._questionStore.add(testCategory);
  }

  update(id: string, question: Partial<Question>) {
    this._questionStore.update(id, question);
  }

  remove(id: ID) {
    this._questionStore.remove(id);
  }

  private get testManagerApiBaseUrl() {
    return this._appSettingService.get<AppSettings>().testManagerApiBaseUrl;
  }
}
