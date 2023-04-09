/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
import { TestDuration } from './test-duration';
export interface ExamStatusWithStep {
  activeQuestion?: ExamQuestion;
  activeQuestionIndex?: null | number;
  examId?: null | string;
  examineeInfo?: null | {
[key: string]: string;
};
  questionCount?: null | number;
  step?: ExamStep;
  testDuration?: TestDuration;
}
