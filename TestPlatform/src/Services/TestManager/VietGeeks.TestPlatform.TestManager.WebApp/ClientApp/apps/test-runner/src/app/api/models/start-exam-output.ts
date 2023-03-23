/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
import { TestDuration } from './test-duration';
export interface StartExamOutput {
  questions?: null | Array<ExamQuestion>;
  startedAt?: string;
  testDuration?: TestDuration;
}
