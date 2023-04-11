/* tslint:disable */
/* eslint-disable */
import { AggregatedGrading } from './aggregated-grading';
export interface FinishExamOutput {
  finalMark?: number;
  grading?: null | Array<AggregatedGrading>;
}
