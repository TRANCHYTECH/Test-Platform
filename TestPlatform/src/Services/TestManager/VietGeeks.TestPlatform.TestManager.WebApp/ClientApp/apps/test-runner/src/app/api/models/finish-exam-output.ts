/* tslint:disable */
/* eslint-disable */
import { AggregatedGrading } from './aggregated-grading';
import { ExamStep } from './exam-step';
export interface FinishExamOutput {
  finalMark?: number;
  grading?: null | Array<AggregatedGrading>;
  step?: ExamStep;
}
