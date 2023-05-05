/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
export interface ActivateQuestionOutput {
  activeQuestion?: ExamQuestion;
  activeQuestionId?: null | string;
  activeQuestionIndex?: null | number;
  canFinish?: boolean;
  canGoToNextQuestion?: boolean;
  canGoToPreviousQuestion?: boolean;
}
