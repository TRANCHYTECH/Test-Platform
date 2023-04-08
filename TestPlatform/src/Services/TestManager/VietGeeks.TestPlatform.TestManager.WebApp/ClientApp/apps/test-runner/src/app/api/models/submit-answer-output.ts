/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
import { ExamStep } from './exam-step';
export interface SubmitAnswerOutput {
  activeQuestion?: ExamQuestion;
  activeQuestionId?: null | string;
  step?: ExamStep;
}
