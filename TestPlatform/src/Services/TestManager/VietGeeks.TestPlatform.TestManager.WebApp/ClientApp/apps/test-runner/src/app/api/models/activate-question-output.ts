/* tslint:disable */
/* eslint-disable */
import { ExamQuestion } from './exam-question';
export interface ActivateQuestionOutput {
  activationResult?: boolean;
  activeQuestion?: ExamQuestion;
  activeQuestionId?: null | string;
  activeQuestionIndex?: null | number;
  answerIds?: null | Array<string>;
  canFinish?: boolean;
}
