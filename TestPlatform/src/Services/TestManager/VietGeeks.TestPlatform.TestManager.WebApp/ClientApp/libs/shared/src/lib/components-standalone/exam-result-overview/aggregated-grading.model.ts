import { ApexNonAxisChartSeries, ApexChart, ApexPlotOptions, ApexFill, ApexStroke } from 'ng-apexcharts';
import { PassMarkGradeOutput } from './pass-mark-grade.model';

export interface AggregatedGrading {
  grades?: null | {
    [key: string]: string;
  };
  gradingType?: number;
  passMarkGrade?: PassMarkGradeOutput;
}

export type ChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  plotOptions: ApexPlotOptions;
  fill: ApexFill;
  stroke: ApexStroke;
  colors: string[];
};