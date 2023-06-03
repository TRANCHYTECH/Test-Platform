/* tslint:disable */
/* eslint-disable */
import { PassMarkGradeOutput } from './pass-mark-grade-output';
export interface AggregatedGradingOuput {
  grades?: null | {
[key: string]: string;
};
  gradingType?: number;
  passMarkGrade?: PassMarkGradeOutput;
}
