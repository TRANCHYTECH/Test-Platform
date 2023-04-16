/* tslint:disable */
/* eslint-disable */
import { AggregatedGrading } from './aggregated-grading';
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
import { TestDuration } from './test-duration';
export interface ExamStatusWithStep {
  activeQuestion?: ExamQuestion;
  activeQuestionIndex?: null | number;
  activeQuestionStartedAt?: null | string;
  examId?: null | string;
  examineeInfo?: null | {
[key: string]: string;
};
  finishededAt?: null | string;
  grading?: null | Array<AggregatedGrading>;
  questionCount?: null | number;
  startedAt?: string;
  step?: ExamStep;
  testDuration?: TestDuration;
  testName?: null | string;
}
