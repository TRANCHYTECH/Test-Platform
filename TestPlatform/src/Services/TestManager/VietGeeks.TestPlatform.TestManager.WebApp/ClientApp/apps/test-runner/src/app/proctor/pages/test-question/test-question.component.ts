import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { ExamQuestion } from '../../../api/models';
import { TestSessionService } from '../../../services/test-session.service';
import { TestDurationMethod, TestSession } from '../../../state/test-session.model';
import { ProctorService } from '../../proctor.service';
@Component({
  selector: 'viet-geeks-test-question',
  templateUrl: './test-question.component.html',
  styleUrls: ['./test-question.component.scss']
})
export class TestQuestionComponent implements OnInit, OnDestroy  {

  proctorService = inject(ProctorService);
  private _testSessionService = inject(TestSessionService);

  questions: ExamQuestion[] = [];
  sessionData: TestSession = {};
  answerForm: FormGroup;
  labels: string[] = [];
  index = 0;
  question?: ExamQuestion;
  endTime: Date = new Date();

  milliSecondsInASecond = 1000;
  hoursInADay = 24;
  minutesInAnHour = 60;
  SecondsInAMinute = 60;

  public timeDifference?: number;
  public secondsToDday?: number;
  public minutesToDday?: number;
  public hoursToDday?: number;
  public daysToDday?: number;

  private subscription?: Subscription;

  constructor(private _fb: FormBuilder) {
    this.answerForm = this._fb.group({
      selectedAnswer: ''
    });
  }

  ngOnInit(): void {
    this.questions = this._testSessionService.getQuestions() ?? [];
    this.question = this.questions[this.index];
    this.sessionData = this._testSessionService.getSessionData();
    this.initEndTime();
    this.subscription = interval(this.milliSecondsInASecond)
      .subscribe(() => this.getTimeDifference());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }

  submit(): void {
    if (this.index < this.questions.length - 1) {
      this.index++;
      this.question = this.questions[this.index];
    }

    this.initEndTime();
  }

  private getTimeDifference() {
    this.timeDifference = this.endTime.getTime() - new Date().getTime();

    if (this.timeDifference <= 0) {
      this.handleEndTime();
    }

    this.allocateTimeUnits(this.timeDifference);
  }

  private allocateTimeUnits(timeDifference: number) {
    this.secondsToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond) % this.SecondsInAMinute);
    this.minutesToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour) % this.SecondsInAMinute);
    this.hoursToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute) % this.hoursInADay);
    this.daysToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute * this.hoursInADay));
  }

  private handleEndTime() {
    if (this.sessionData?.timeSettings?.method == TestDurationMethod.CompleteTestTime) {
      // TODO: navigate to Finish
    } else {
      this.submit();
    }
  }

  private initEndTime() {
    const timeSettings = this.sessionData.timeSettings;
    const durationInMinutes = (timeSettings?.duration?.hours ?? 0) * 60 + (timeSettings?.duration?.minutes ?? 0);

    if (timeSettings?.method == TestDurationMethod.CompleteTestTime) {
      const startTime = this.sessionData.startTime ?? new Date();
      this.endTime = new Date();
      this.endTime.setMinutes(startTime.getMinutes() + durationInMinutes);
    }
    else {
      this.endTime = new Date();
      this.endTime.setMinutes(this.endTime.getMinutes() + durationInMinutes);
    }
  }
}
