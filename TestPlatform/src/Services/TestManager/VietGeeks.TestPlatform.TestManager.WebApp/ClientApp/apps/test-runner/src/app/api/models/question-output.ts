/* tslint:disable */
/* eslint-disable */
import { AnswerOutput } from './answer-output';
export interface QuestionOutput {
  answerType?: number;
  categoryId?: null | string;
  description?: null | string;
  id?: null | string;
  questionAnswers?: null | Array<AnswerOutput>;
  questionNo?: number;
}
