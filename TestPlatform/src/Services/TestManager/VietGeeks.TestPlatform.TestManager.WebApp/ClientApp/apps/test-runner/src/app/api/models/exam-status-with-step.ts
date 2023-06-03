/* tslint:disable */
/* eslint-disable */
import { AggregatedGradingOuput } from './aggregated-grading-ouput';
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
import { TestDuration } from './test-duration';
import { TimeSpan } from './time-span';
export interface ExamStatusWithStep {
  activeQuestion?: ExamQuestion;
  activeQuestionIndex?: null | number;
  activeQuestionStartedAt?: null | string;
  canFinish?: boolean;
  canSkipQuestion?: boolean;
  examId?: null | string;
  examineeInfo?: null | {
[key: string]: string;
};
  finishededAt?: null | string;
  grading?: null | Array<AggregatedGradingOuput>;
  questionCount?: null | number;
  startedAt?: string;
  step?: ExamStep;
  testDuration?: TestDuration;
  testName?: null | string;
  totalDuration?: TimeSpan;
}
