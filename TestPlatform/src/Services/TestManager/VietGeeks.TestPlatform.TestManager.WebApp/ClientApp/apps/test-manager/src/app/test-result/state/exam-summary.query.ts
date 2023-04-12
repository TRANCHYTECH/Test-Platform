import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { ExamSummaryStore, ExamSummaryState } from './exam-summary.store';

@Injectable({ providedIn: 'root' })
export class ExamSummaryQuery extends QueryEntity<ExamSummaryState> {

  constructor(protected store: ExamSummaryStore) {
    super(store);
  }

}
