import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { TestCategoriesStore, TestCategoriesState } from './test-categories.store';

@Injectable({ providedIn: 'root' })
export class TestCategoriesQuery extends QueryEntity<TestCategoriesState> {

  constructor(store: TestCategoriesStore) {
    super(store);
  }

}
