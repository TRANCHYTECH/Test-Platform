import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { TestSessionState, TestSessionStore } from './test-session.store';

@Injectable({ providedIn: 'root' })
export class TestSessionQuery extends QueryEntity<TestSessionState> {

  constructor(store: TestSessionStore) {
    super(store);
  }
}
