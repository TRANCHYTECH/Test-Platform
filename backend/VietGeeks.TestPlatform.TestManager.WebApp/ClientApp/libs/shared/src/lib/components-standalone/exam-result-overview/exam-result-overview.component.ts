import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AggregatedGrading, ChartOptions } from './aggregated-grading.model';
import { GradingCriteriaConfigType, RangeUnit } from '../../models/test.model';
import createChartOptions from './chart-options.factory';

@Component({
  selector: 'viet-geeks-exam-result-overview',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './exam-result-overview.component.html',
  styleUrls: ['./exam-result-overview.component.scss']
})
export class ExamResultOverviewComponent {
  @Input() containerClass = '';
  @Input({ required: true })
  set gradings(value: AggregatedGrading[] | undefined) {
    this.passMarkGrading = value?.filter(g => g.gradingType == GradingCriteriaConfigType.PassMask)[0];

    const gradeRangesGrading = value?.filter(g => g.gradingType == GradingCriteriaConfigType.GradeRanges)[0];
    if (gradeRangesGrading && gradeRangesGrading?.grades) {
      this.gradeRangesValues = Object.values(gradeRangesGrading.grades);
    } else {
      this.gradeRangesValues = [];
    }

    if (this.passMarkGrading) {
      const color = this.passMarkGrading?.passMarkGrade?.isPass ? '#20E647' : '#ff7067';
      const chartValues = this.calculateChartValues();
      const unit = this.passMarkGrading?.passMarkGrade?.unit == RangeUnit.Percent ? 'Percent' : 'Point';
      this.chartOptions = createChartOptions(chartValues, unit, color);
    }
  }

  passMarkGrading?: AggregatedGrading;
  gradeRangesValues?: string[];
  chartOptions?: ChartOptions;

  private calculateChartValues(): { finalPoints: number, totalPoints: number, percentage: number} {
    const finalPoints = this.passMarkGrading?.passMarkGrade?.finalPoints ?? 0;
    const totalPoints = this.passMarkGrading?.passMarkGrade?.totalPoints ?? 0;
    const values = {
      finalPoints,
      totalPoints,
      percentage: 0
    };

    if (this.passMarkGrading?.passMarkGrade?.unit == RangeUnit.Point) {
      if (totalPoints == 0) {
        return values;
      }

      values.percentage = Math.floor(finalPoints / totalPoints * 100);

      return values;
    }

    values.percentage = finalPoints;

    return values;
  }
}

