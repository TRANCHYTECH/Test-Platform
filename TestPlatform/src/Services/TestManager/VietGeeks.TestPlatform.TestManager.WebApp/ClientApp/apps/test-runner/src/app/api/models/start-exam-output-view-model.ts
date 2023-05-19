/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
import { TestDuration } from './test-duration';
export interface StartExamOutputViewModel {
  activeQuestion?: ExamQuestion;
  activeQuestionIndex?: null | number;
  canSkipQuestion?: boolean;
  startedAt?: string;
  step?: ExamStep;
  testDuration?: TestDuration;
  totalQuestion?: number;
}
