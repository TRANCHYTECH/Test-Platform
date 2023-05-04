import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { interval, Subscription, firstValueFrom } from 'rxjs';
import { ExamQuestion, TimeSpan } from '../../../api/models';
import { AnswerType } from '../../../state/exam-content.model';
import { ExamCurrentStep, TestDurationMethod, TestSession } from '../../../state/test-session.model';
import { ProctorService } from '../../services/proctor.service';
import { TestDurationService } from '../../services/test-duration.service';
import { TestSessionService } from '../../services/test-session.service';
import { ToastService } from '@viet-geeks/shared';
@Component({
  selector: 'viet-geeks-test-question',
  templateUrl: './test-question.component.html',
  styleUrls: ['./test-question.component.scss']
})
export class TestQuestionComponent implements OnInit, OnDestroy  {

  proctorService = inject(ProctorService);
  private _testDurationService = inject(TestDurationService);
  private _testSessionService = inject(TestSessionService);
  private _notifyService = inject(ToastService);
  private _router = inject(Router);

  questions: ExamQuestion[] = [];
  sessionData: TestSession = {};
  answerForm: FormGroup;
  labels: string[] = [];
  index = 0;
  question?: ExamQuestion;
  questionCount = 0;
  endTime: Date = new Date();
  remainingTime: TimeSpan = {};
  private subscription?: Subscription;

  constructor(private _fb: FormBuilder) {
    this.answerForm = this._fb.group({});
  }

  ngOnInit() {
    this.sessionData = this._testSessionService.getSessionData();
    this.index = this.sessionData.questionIndex ?? 0;
    this.question = this.sessionData.activeQuestion;
    this.questionCount = this.sessionData.questionCount ?? 0;
    this.initAnswerForm();
    this.initEndTime();
    this.subscription = interval(1000)
      .subscribe(() => this.getTimeDifference());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }

  async submit() {
    if (this.question) {
      const submitAnswerOutput = await firstValueFrom(this.proctorService.submitAnswer({
        questionId: this.question.id!,
        answerIds: this.getAnswerIds()
      }));

      if (submitAnswerOutput != null) {
        this._testSessionService.setSessionData({
          examStep: ExamCurrentStep.SubmitAnswer,
          activeQuestion: submitAnswerOutput.activeQuestion,
          activeQuestionStartAt: null,
          questionIndex: submitAnswerOutput.activeQuestionIndex ?? undefined
        });
        this.question = submitAnswerOutput.activeQuestion;
        this.index = submitAnswerOutput.activeQuestionIndex ?? this.index;
        if (this.question) {
          this.initAnswerForm();
          this.initEndTime();
        }
        else {
          this.finishExam();
        }
      }
      else {
        this.finishExam();
      }
    }
  }

  get submitEnabled() {
    return !this.question?.scoreSettings?.mustAnswerToContinue || this.answerForm.dirty;
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
      this.handleTimeUp();
    }

    this.allocateTimeUnits(timeDifference);
  }

  private allocateTimeUnits(timeDifference: number) {
    this.remainingTime = this._testDurationService.getDurationFromTimeDifference(timeDifference);
  }

  private async handleTimeUp() {
    await this._notifyService.warning('Time is up. The answer will be submitted automatically.');

    if (this.sessionData?.timeSettings?.method == TestDurationMethod.CompleteTestTime) {
      await this.finishExam();
    } else {
      this.submit();
    }
  }

  private async finishExam() {
    const finishOutput = await firstValueFrom(this.proctorService.finishExam());
    this._testSessionService.setSessionData({
      endTime: finishOutput?.finishedAt ? new Date(finishOutput?.finishedAt) : new Date(),
      examStep: ExamCurrentStep.FinishExam,
      grading: finishOutput?.grading,
      questions: finishOutput?.questions,
      answers: finishOutput?.examAnswers,
      questionScores: finishOutput?.questionScores
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
      let startTime;
      if (this.sessionData.activeQuestionStartAt) {
        startTime = new Date(this.sessionData.activeQuestionStartAt);
      }
      else {
        startTime = new Date();
      }

      this.endTime = new Date(startTime.getTime() + durationInMinutes * 60000);
    }
  }
}
