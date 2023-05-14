import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Question } from '@viet-geeks/shared';

export type QuestionsState = EntityState<Question>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'Questions' })
export class QuestionStore extends EntityStore<QuestionsState> {

  constructor() {
    super();
  }

}
