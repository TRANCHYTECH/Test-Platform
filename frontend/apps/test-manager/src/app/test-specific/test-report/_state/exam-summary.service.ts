import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ExamReview,
  ExamSummary,
  Respondent,
  TestRunSummary,
} from './exam-summary.model';
import { ExamSummaryStore } from './exam-summary.store';
import { injectPortalConfig } from '@viet-geeks/core';
import { PortalConfig } from '../../../app.config';

@Injectable({ providedIn: 'root' })
export class ExamSummaryService {
  constructor(
    private examSummaryStore: ExamSummaryStore,
    private http: HttpClient
  ) {}

  getTestRuns(testId: string) {
    return this.http.get<TestRunSummary[]>(
      `/TestManager:/Management/TestReport/${testId}/TestRuns`
    );
  }

  get(testRunIds: string[]) {
    return this.http.get<ExamSummary[]>(
      `/TestManager:/Management/TestReport/Exams`,
      { params: { testRunIds: testRunIds } }
    );
  }

  getRespondents(testRunIds: string[]) {
    return this.http.get<Respondent[]>(
      `/TestManager:/Management/TestReport/Respondents`,
      { params: { testRunIds: testRunIds } }
    );
  }

  getExamReview(examId: string) {
    return this.http.get<ExamReview>(
      `/TestManager:/Management/TestReport/Exam/${examId}/Review`
    );
  }
}
