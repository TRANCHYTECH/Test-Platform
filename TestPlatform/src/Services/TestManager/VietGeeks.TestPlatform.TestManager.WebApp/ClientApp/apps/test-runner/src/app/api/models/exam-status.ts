/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
export interface ExamStatus {
  activeQuestion?: ExamQuestion;
  examId?: null | string;
  examineeInfo?: null | {
[key: string]: string;
};
  previousStep?: ExamStep;
}
