import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { QuestionsState, QuestionsStore } from './question.store';

@Injectable({ providedIn: 'root' })
export class QuestionsQuery extends QueryEntity<QuestionsState> {

  constructor(store: QuestionsStore) {
    super(store);
  }
}
