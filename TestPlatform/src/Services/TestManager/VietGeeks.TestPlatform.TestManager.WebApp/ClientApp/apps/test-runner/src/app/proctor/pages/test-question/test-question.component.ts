import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { ExamQuestion } from '../../../api/models';
import { TestSessionService } from '../../../services/test-session.service';
import { TestSession } from '../../../state/test-session.model';
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
  sessionData?: TestSession;
  answerForm: FormGroup;
  labels: string[] = [];
  index = 0;
  question?: ExamQuestion;
  endTime: Date;

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

    this.endTime = new Date();
    this.endTime.setMinutes(this.endTime.getMinutes() + 30);
  }

  ngOnInit(): void {
    this.questions = this._testSessionService.getQuestions() ?? [];
    this.sessionData = this._testSessionService.getSessionData();
    this.question = this.questions[this.index];
    this.subscription = interval(1000)
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
  }

  private getTimeDifference() {
    this.timeDifference = this.endTime.getTime() - new Date().getTime();
    this.allocateTimeUnits(this.timeDifference);
  }

  private allocateTimeUnits(timeDifference: number) {
    this.secondsToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond) % this.SecondsInAMinute);
    this.minutesToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour) % this.SecondsInAMinute);
    this.hoursToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute) % this.hoursInADay);
    this.daysToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute * this.hoursInADay));
  }
}
