import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { QuestionCategoriesStore, QuestionCategoriesState } from './question-categories.store';
import { QuestionCategoryGenericId } from './question-categories.model';

@Injectable({ providedIn: 'root' })
export class QuestionCategoriesQuery extends QueryEntity<QuestionCategoriesState> {

  constructor(store: QuestionCategoriesStore) {
    super(store);
  }

  getEntityWithFallback(id: string) {
    let entity = this.getEntity(id);
    if(entity === undefined)
      entity = this.getEntity(QuestionCategoryGenericId);

    return entity;
  }
}
