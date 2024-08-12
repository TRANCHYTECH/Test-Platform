/* tslint:disable */
/* eslint-disable */
import { AggregatedGradingOuput } from './aggregated-grading-ouput';
import { QuestionOutput } from './question-output';
import { TimeSpan } from './time-span';
export interface FinishExamOutput {
  examAnswers?: null | {
[key: string]: Array<string>;
};
  finalMark?: number;
  finishedAt?: string;
  grading?: null | Array<AggregatedGradingOuput>;
  questionScores?: null | {
[key: string]: number;
};
  questions?: null | Array<QuestionOutput>;
  totalTime?: TimeSpan;
}
