/* tslint:disable */
/* eslint-disable */
import { AggregatedGrading } from './aggregated-grading';
export interface FinishExamOutput {
  finalMark?: number;
  finishedAt?: string;
  grading?: null | Array<AggregatedGrading>;
}
