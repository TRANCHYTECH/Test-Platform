import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { interval, Subscription, firstValueFrom } from 'rxjs';
import { ExamQuestion } from '../../../api/models';
import { TestSessionService } from '../../../services/test-session.service';
import { AnswerType } from '../../../state/exam-content.model';
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
    this.sessionData = this._testSessionService.getSessionData();
    this.answerForm = this._fb.group({});
    this.questions = this._testSessionService.getQuestions() ?? [];
    this.question = this.questions[this.index];

    this.initAnswerForm();
  }

  ngOnInit(): void {
    this.initEndTime();
    this.subscription = interval(this.milliSecondsInASecond)
      .subscribe(() => this.getTimeDifference());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }

  async submit() {
    if (this.question) {
      await firstValueFrom(this.proctorService.submitAnswer({
        questionId: this.question.id!,
        answerIds: this.getAnswerIds()
      }));
    }

    if (this.index < this.questions.length - 1) {
      this.index++;
      this.question = this.questions[this.index];
      this.initAnswerForm();
      this.initEndTime();
    }
  }

  private getAnswerIds(): string[] {
    if (this.question!.answerType == AnswerType.SingleChoice) {
      return [(this.answerForm.value as { selectedAnswer: string; }).selectedAnswer];
    }

    return ((this.answerForm.value as { selectedAnswer: {id: string, selected: boolean}[]; }).selectedAnswer).filter((i) => i.selected).map(i => i.id);
  }

  private initAnswerForm() {
    if (this.question!.answerType == AnswerType.SingleChoice) {
      this.answerForm = this._fb.group({
        selectedAnswer: 0
      });
    }
    else {
      const selectedAnswerForms = this._fb.array([]) as FormArray;

      this.question!.answers?.forEach(answer => {
        const formGroup = this._fb.group({
          id: answer.id,
          selected: false
        });
        selectedAnswerForms.push(formGroup);
      });

      this.answerForm = this._fb.group({
        selectedAnswer: selectedAnswerForms
      });
    }
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
