import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Question } from './question.model';

export type QuestionsState = EntityState<Question>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'Questions' })
export class QuestionStore extends EntityStore<QuestionsState> {

  constructor() {
    super();
  }

}
