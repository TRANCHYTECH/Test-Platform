import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { TestSession } from './test-session.model';

export type TestSessionState = EntityState<TestSession>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'TestSession' })
export class TestSessionStore extends EntityStore<TestSessionState> {

  constructor() {
    super();
  }

}
