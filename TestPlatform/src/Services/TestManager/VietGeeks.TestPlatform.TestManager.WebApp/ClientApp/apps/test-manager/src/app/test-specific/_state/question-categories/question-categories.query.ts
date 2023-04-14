import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { QuestionCategoriesStore, QuestionCategoriesState } from './question-categories.store';

@Injectable({ providedIn: 'root' })
export class QuestionCategoriesQuery extends QueryEntity<QuestionCategoriesState> {

  constructor(store: QuestionCategoriesStore) {
    super(store);
  }

}
