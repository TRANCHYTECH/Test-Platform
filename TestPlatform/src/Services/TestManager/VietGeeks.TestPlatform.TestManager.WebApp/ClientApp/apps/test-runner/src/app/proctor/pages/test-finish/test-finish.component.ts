import { Component, OnInit, inject } from '@angular/core';
import { ChartOptions } from '@viet-geeks/shared';
import { firstValueFrom } from 'rxjs';
import { AfterTestConfigOutput, AggregatedGradingOuput, FinishExamOutput, TimeSpan } from '../../../api/models';
import { GradingCriteriaConfigType, InformFactor, RespondentField, TestSession } from '../../../state/test-session.model';
import { ProctorService } from '../../services/proctor.service';
import { TestDurationService } from '../../services/test-duration.service';
import { TestSessionService } from '../../services/test-session.service';

@Component({
  selector: 'viet-geeks-test-finish',
  templateUrl: './test-finish.component.html',
  styleUrls: ['./test-finish.component.scss']
})
export class TestFinishComponent implements OnInit {
  private _proctorService = inject(ProctorService);
  private _testDurationService = inject(TestDurationService);
  private _testSessionService = inject(TestSessionService);

  labels: string[] = [];
  sessionData: Partial<TestSession> = {};
  respondent: RespondentField[] = [];
  testResult?: FinishExamOutput | null;
  maxTime: TimeSpan = {};
  totalTime: TimeSpan = {};
  chartOptions?: ChartOptions;
  passMarkGrading?: AggregatedGradingOuput;
  afterTestConfig?: AfterTestConfigOutput | null;
  showPassFailMessage = false;
  showCorrectAnswers = false;

  ngOnInit() {
    this.doInit();
  }

  private async doInit() {
    await this.setupData();
  }

  private async setupData() {
    this.setupSessionData();
    this.afterTestConfig = await firstValueFrom(this._proctorService.getAfterTestConfig());

    if (this.afterTestConfig?.testEndConfig?.redirectTo && this.afterTestConfig.testEndConfig.toAddress) {
      location.href = this.afterTestConfig.testEndConfig.toAddress;
      return;
    }

    const informFactors = this.afterTestConfig?.informRespondentConfig?.informFactors;
    if (informFactors) {
      this.showPassFailMessage = informFactors[InformFactor.PassOrFailMessage];
      this.showCorrectAnswers = informFactors[InformFactor.CorrectAnwsers];
    }
  }

  private setupSessionData() {
    this.sessionData = this._testSessionService.getSessionData();
    this.respondent = this.sessionData.respondentFields ?? [];

    const gradings = this.sessionData.grading;
    this.passMarkGrading = gradings?.filter(g => g.gradingType == GradingCriteriaConfigType.PassMask)[0];
    this.totalTime = this._testDurationService.getDuration(this.sessionData.startTime, this.sessionData.endTime);
    this.maxTime = this._testDurationService.getMaximumTime(this.sessionData.timeSettings, this.sessionData.questionCount);
  }

}
