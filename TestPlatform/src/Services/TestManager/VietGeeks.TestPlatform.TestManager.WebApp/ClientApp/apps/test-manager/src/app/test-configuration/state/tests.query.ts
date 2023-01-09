import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { TestsStore, TestsState } from './tests.store';

@Injectable({ providedIn: 'root' })
export class TestsQuery extends QueryEntity<TestsState> {

  constructor(store: TestsStore) {
    super(store);
  }
}
