import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { ExamSummaryStore, ExamSummaryState } from './exam-summary.store';

@Injectable({ providedIn: 'root' })
export class ExamSummaryQuery extends Query<ExamSummaryState> {

  constructor(protected store: ExamSummaryStore) {
    super(store);
  }

}
