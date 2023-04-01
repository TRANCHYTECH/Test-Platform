import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { interval, Subscription, firstValueFrom } from 'rxjs';
import { ExamQuestion, TimeSpan } from '../../../api/models';
import { TestSessionService } from '../../services/test-session.service';
import { AnswerType } from '../../../state/exam-content.model';
import { TestDurationMethod, TestSession } from '../../../state/test-session.model';
import { ProctorService } from '../../services/proctor.service';
import { TestDurationService } from '../../services/test-duration.service';
@Component({
  selector: 'viet-geeks-test-question',
  templateUrl: './test-question.component.html',
  styleUrls: ['./test-question.component.scss']
})
export class TestQuestionComponent implements OnInit, OnDestroy  {

  proctorService = inject(ProctorService);
  private _testSessionService = inject(TestSessionService);
  private _testDurationService = inject(TestDurationService);
  private _router = inject(Router);

  questions: ExamQuestion[] = [];
  sessionData: TestSession = {};
  answerForm: FormGroup;
  labels: string[] = [];
  index = 0;
  question?: ExamQuestion;
  endTime: Date = new Date();

  public remainingTime: TimeSpan = {};

  private subscription?: Subscription;

  constructor(private _fb: FormBuilder) {
    this.sessionData = this._testSessionService.getSessionData();
    this.index = this.sessionData.questionIndex ?? 0;
    this.answerForm = this._fb.group({});
    this.questions = this._testSessionService.getQuestions() ?? [];
    this.question = this.questions[this.index];

    this.initAnswerForm();
  }

  ngOnInit(): void {
    this.initEndTime();
    this.subscription = interval(1000)
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
    else {
      this.finishExam();
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
    const timeDifference = this._testDurationService.getTimeDifference(new Date(), this.endTime);
    if (timeDifference <= 0) {
      this.handleEndTime();
    }

    this.allocateTimeUnits(timeDifference);
  }

  private allocateTimeUnits(timeDifference: number) {
    this.remainingTime = this._testDurationService.getDurationFromTimeDifference(timeDifference);
  }

  private async handleEndTime() {
    if (this.sessionData?.timeSettings?.method == TestDurationMethod.CompleteTestTime) {
      await this.finishExam();
    } else {
      this.submit();
    }
  }

  private async finishExam() {
    const finishOutput = await firstValueFrom(this.proctorService.finishExam());
    this._testSessionService.setSessionData({
      result: finishOutput,
      endTime: new Date()
    });
    this._router.navigate(['test/finish']);
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
