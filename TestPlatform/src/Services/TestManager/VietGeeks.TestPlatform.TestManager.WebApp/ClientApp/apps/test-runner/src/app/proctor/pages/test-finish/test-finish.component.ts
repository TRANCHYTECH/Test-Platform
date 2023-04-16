import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AggregatedGrading, FinishExamOutput, TimeSpan } from '../../../api/models';
import { TestSessionService } from '../../services/test-session.service';
import { GradingCriteriaConfigType, RespondentField, TestSession } from '../../../state/test-session.model';
import { TestDurationService } from '../../services/test-duration.service';
import { ApexChart, ApexFill, ApexNonAxisChartSeries, ApexPlotOptions, ApexStroke, ChartComponent } from 'ng-apexcharts';
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
  isPass = false;
  maxTime: TimeSpan = {};
  totalTime: TimeSpan = {};
  public chartOptions: ChartOptions;

  public passMarkGrading?: AggregatedGrading;
  public gradeRangesGrading?: AggregatedGrading;

  constructor() {
    this.setupSessionData();

    if (this.passMarkGrading) {
    }

    // todo: Map percentage from API
    const percentage = Math.random() * 100;

    const color = this.isPass ? '#20E647' : '#ff7067';

    this.chartOptions = {
      series: [percentage],
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
                return parseInt(val.toString(), 10).toString();
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
      labels: ["Percent"],
      colors: [color]
    };

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
