/* tslint:disable */
/* eslint-disable */
import { AggregatedGradingOuput } from './aggregated-grading-ouput';
import { QuestionOutput } from './question-output';
export interface FinishExamOutput {
  examAnswers?: null | {
[key: string]: Array<string>;
};
  finalMark?: number;
  finishedAt?: string;
  grading?: null | Array<AggregatedGradingOuput>;
  questions?: null | Array<QuestionOutput>;
}
