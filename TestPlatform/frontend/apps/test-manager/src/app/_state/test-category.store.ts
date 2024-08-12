import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { TestCategory } from './test-category.model';

export type TestCategoryState = EntityState<TestCategory>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'TestCategory' })
export class TestCategoryStore extends EntityStore<TestCategoryState> {

  constructor() {
    super();
  }

}
