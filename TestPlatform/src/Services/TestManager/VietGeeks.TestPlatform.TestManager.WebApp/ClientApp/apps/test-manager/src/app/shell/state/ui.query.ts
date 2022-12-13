import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { UiStore, UiState } from './ui.store';

@Injectable({ providedIn: 'root' })
export class UiQuery extends QueryEntity<UiState> {

  constructor(protected override store: UiStore) {
    super(store);
  }

  selectMainMenus() {
    return this.selectEntity('main', c => c?.routes || []);
  }
}
