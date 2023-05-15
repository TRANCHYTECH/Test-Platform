import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AggregatedGrading, ChartOptions } from './aggregated-grading.model';
import { GradingCriteriaConfigType, RangeUnit } from '../../models/test.model';

@Component({
  selector: 'viet-geeks-exam-result-overview',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './exam-result-overview.component.html',
  styleUrls: ['./exam-result-overview.component.scss']
})
export class ExamResultOverviewComponent {
  
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

      this.chartOptions = {
        series: [chartValues.percentage],
        chart: {
          height: 300,
          type: "radialBar",
        },
        plotOptions: {
          radialBar: {
            startAngle: -135,
            endAngle: 225,
            hollow: {
              margin: 0,
              size: "70%",
              background: "#fff",
              image: undefined,
              position: "front",
              dropShadow: {
                enabled: true,
                top: 3,
                left: 0,
                blur: 4,
                opacity: 0.24
              }
            },
            track: {
              background: "#fff",
              strokeWidth: "67%",
              margin: 0,
              dropShadow: {
                enabled: true,
                top: -3,
                left: 0,
                blur: 4,
                opacity: 0.35
              }
            },

            dataLabels: {
              show: true,
              name: {
                offsetY: -10,
                show: true,
                color: "#888",
                fontSize: "17px"
              },
              value: {
                formatter: function (val) {
                  if (unit == 'Percent') {
                    return parseInt(val.toString(), 10).toString();
                  }

                  return `${chartValues.finalPoints} / ${chartValues.totalPoints}`;
                },
                color: "#111",
                fontSize: "36px",
                show: true
              }
            }
          }
        },
        fill: {
          type: "basic",
          gradient: {
            shade: "dark",
            type: "horizontal",
            shadeIntensity: 0.5,
            gradientToColors: ["#ff7067"],
            inverseColors: false,
            opacityFrom: 1,
            opacityTo: 1,
            stops: [0, 100]
          }
        },
        stroke: {
          lineCap: "round"
        },
        labels: [unit],
        colors: [color]
      };
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
