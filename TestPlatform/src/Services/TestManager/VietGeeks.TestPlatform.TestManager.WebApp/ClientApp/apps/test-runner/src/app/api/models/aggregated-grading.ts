/* tslint:disable */
/* eslint-disable */
import { PassMarkGrade } from './pass-mark-grade';
export interface AggregatedGrading {
  grades?: null | {
[key: string]: string;
};
  gradingType?: number;
  passMarkGrade?: PassMarkGrade;
}
