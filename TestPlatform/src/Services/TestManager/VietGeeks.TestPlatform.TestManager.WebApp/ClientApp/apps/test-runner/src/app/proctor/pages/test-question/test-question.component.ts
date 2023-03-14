import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { TestRespondentField } from '../../../state/test.model';
import { ProctorService } from '../../proctor.service';
import { TestQuestions, TestTakerMetaData } from './data';
@Component({
  selector: 'viet-geeks-test-question',
  templateUrl: './test-question.component.html',
  styleUrls: ['./test-question.component.scss']
})
export class TestQuestionComponent implements OnInit, OnDestroy  {

  proctorService = inject(ProctorService);
  questions = TestQuestions;
  metaData = TestTakerMetaData;
  answerForm: FormGroup;
  labels: string[] = [];
  index = 0;
  question: any;
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
    this.question = TestQuestions[this.index];
    this.subscription = interval(1000)
      .subscribe(() => this.getTimeDifference());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }

  submit(): void {
    this.index++;
    this.question = TestQuestions[this.index];
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
