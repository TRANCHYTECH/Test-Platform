import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { TestCategory } from './test-category.model';

export type TestCategoriesState = EntityState<TestCategory>

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'TestCategories' })
export class TestCategoriesStore extends EntityStore<TestCategoriesState> {

  constructor() {
    super();
  }

}
