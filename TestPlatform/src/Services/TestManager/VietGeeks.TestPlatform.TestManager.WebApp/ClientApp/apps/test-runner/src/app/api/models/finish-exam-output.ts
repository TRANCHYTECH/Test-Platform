/* tslint:disable */
/* eslint-disable */
import { AggregatedGradingOuput } from './aggregated-grading-ouput';
export interface FinishExamOutput {
  finalMark?: number;
  finishedAt?: string;
  grading?: null | Array<AggregatedGradingOuput>;
}
