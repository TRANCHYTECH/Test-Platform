import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { TestCategoryStore, TestCategoryState } from './test-category.store';
import { TestCategoryUncategorizedId } from './test-category.model';

@Injectable({ providedIn: 'root' })
export class TestCategoryQuery extends QueryEntity<TestCategoryState> {

  constructor(store: TestCategoryStore) {
    super(store);
  }

  getEntityWithFallback(id: string) {
    let entity = this.getEntity(id);
    if(entity === undefined)
      entity = this.getEntity(TestCategoryUncategorizedId);

    return entity;
  }
}
