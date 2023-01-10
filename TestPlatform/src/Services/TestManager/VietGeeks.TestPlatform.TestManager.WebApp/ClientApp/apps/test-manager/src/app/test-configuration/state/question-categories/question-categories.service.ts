import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { tap } from 'rxjs/operators';
import { QuestionCategory } from './question-categories.model';
import { QuestionCategoriesStore } from './question-categories.store';

@Injectable({ providedIn: 'root' })
export class QuestionCategoriesService {

  constructor(private questionCategoriesStore: QuestionCategoriesStore, private http: HttpClient) {
  }

  get() {
    return this.http.get<QuestionCategory[]>('https://api.com').pipe(tap(entities => {
      this.questionCategoriesStore.set(entities);
    }));
  }

  add(testCategory: QuestionCategory) {
    this.questionCategoriesStore.add(testCategory);
  }

  update(id: string, questionCategory: Partial<QuestionCategory>) {
    this.questionCategoriesStore.update(id, questionCategory);
  }

  remove(id: ID) {
    this.questionCategoriesStore.remove(id);
  }

}
