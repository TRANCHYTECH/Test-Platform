import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface ExamSummaryState {
   key: string;
}

export function createInitialState(): ExamSummaryState {
  return {
    key: ''
  };
}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'ExamSummary' })
export class ExamSummaryStore extends Store<ExamSummaryState> {

  constructor() {
    super(createInitialState());
  }

}
