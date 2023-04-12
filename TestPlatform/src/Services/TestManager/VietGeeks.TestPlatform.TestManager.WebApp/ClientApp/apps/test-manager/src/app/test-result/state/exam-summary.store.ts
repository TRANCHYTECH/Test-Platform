import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { ExamSummary } from './exam-summary.model';

export interface ExamSummaryState extends EntityState<ExamSummary> {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'ExamSummary' })
export class ExamSummaryStore extends EntityStore<ExamSummaryState> {

  constructor() {
    super();
  }

}
