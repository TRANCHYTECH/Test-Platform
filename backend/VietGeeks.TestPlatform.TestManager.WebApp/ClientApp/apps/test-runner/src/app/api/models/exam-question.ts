/* tslint:disable */
/* eslint-disable */
import { ExamAnswer } from './exam-answer';
import { ExamQuestionScoreSettings } from './exam-question-score-settings';
export interface ExamQuestion {
  answerType?: number;
  answers?: null | Array<ExamAnswer>;
  description?: null | string;
  id?: null | string;
  scoreSettings?: ExamQuestionScoreSettings;
}
