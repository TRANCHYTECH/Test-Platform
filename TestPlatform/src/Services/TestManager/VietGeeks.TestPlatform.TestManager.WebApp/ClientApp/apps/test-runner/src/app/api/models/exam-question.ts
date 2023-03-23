/* tslint:disable */
/* eslint-disable */
import { ExamAnswer } from './exam-answer';
export interface ExamQuestion {
  answerType?: number;
  answers?: null | Array<ExamAnswer>;
  description?: null | string;
  id?: null | string;
}
