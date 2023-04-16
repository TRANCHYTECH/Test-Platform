import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AggregatedGrading, FinishExamOutput, TimeSpan } from '../../../api/models';
import { GradingCriteriaConfigType, RangeUnit, RespondentField, TestSession } from '../../../state/test-session.model';
import { TestDurationService } from '../../services/test-duration.service';
import { ApexChart, ApexFill, ApexNonAxisChartSeries, ApexPlotOptions, ApexStroke } from 'ng-apexcharts';
import { TestSessionQuery } from '../../../state/test-session.query';

export type ChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  plotOptions: ApexPlotOptions;
  fill: ApexFill;
  stroke: ApexStroke;
  colors: any
};

@Component({
  selector: 'viet-geeks-test-finish',
  templateUrl: './test-finish.component.html',
  styleUrls: ['./test-finish.component.scss']
})
export class TestFinishComponent {

  private _testDurationService = inject(TestDurationService);
  private _testSessionQuery = inject(TestSessionQuery);
  router = inject(Router);
  labels: string[] = [];
  sessionData: Partial<TestSession> = {};
  respondent: RespondentField[] = [];
  testResult?: FinishExamOutput | null;
  maxTime: TimeSpan = {};
  totalTime: TimeSpan = {};
  public chartOptions?: ChartOptions;
  public passMarkGrading?: AggregatedGrading;
  public gradeRangesGrading?: AggregatedGrading;

  constructor() {
    this.setupSessionData();
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
              margin: 0, // margin is in pixels
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
                formatter: function(val) {
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

  private setupSessionData() {
    this.sessionData = this._testSessionQuery.getEntity(1) ?? {};
    this.respondent = this.sessionData.respondentFields ?? [];
    // this.testResult = this.sessionData.result;

    const gradings = this.sessionData.grading;

    // this.isPass = (!!this.testResult?.grading) && this.testResult?.grading[0] && this.testResult?.grading[0].passMark == true;

    this.passMarkGrading = gradings?.filter(g => g.gradingType == GradingCriteriaConfigType.PassMask)[0];
    this.gradeRangesGrading = gradings?.filter(g => g.gradingType == GradingCriteriaConfigType.GradeRanges)[0];

    this.totalTime = this._testDurationService.getDuration(this.sessionData.startTime, this.sessionData.endTime);
    this.maxTime = this._testDurationService.getMaximumTime(this.sessionData.timeSettings, this.sessionData.questionCount);
  }

}
