import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Test } from './test.model';

export interface TestsState extends EntityState<Test, string>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'Tests' })
export class TestsStore extends EntityStore<TestsState> {

  constructor() {
    super();
  }

}
