import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AggregatedGrading, ChartOptions } from './aggregated-grading.model';

@Component({
  selector: 'viet-geeks-exam-result-overview',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './exam-result-overview.component.html',
  styleUrls: ['./exam-result-overview.component.scss']
})
export class ExamResultOverviewComponent {
  @Input()
  passMarkGrading?: AggregatedGrading;

  @Input()
  gradeRangesValues?: string[];

  @Input()
  chartOptions?: ChartOptions;
}
