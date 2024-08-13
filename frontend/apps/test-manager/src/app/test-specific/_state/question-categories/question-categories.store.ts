import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { QuestionCategory } from './question-categories.model';

export type QuestionCategoriesState = EntityState<QuestionCategory>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'QuestionCategories' })
export class QuestionCategoriesStore extends EntityStore<QuestionCategoriesState> {

  constructor() {
    super();
  }

}
