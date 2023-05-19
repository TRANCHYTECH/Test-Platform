import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { interval, Subscription, firstValueFrom } from 'rxjs';
import { ActivateQuestionOutput, ExamQuestion, TimeSpan } from '../../../api/models';
import { AnswerType } from '../../../state/exam-content.model';
import { ExamCurrentStep, TestDurationMethod, TestSession } from '../../../state/test-session.model';
import { ProctorService } from '../../services/proctor.service';
import { TestDurationService } from '../../services/test-duration.service';
import { TestSessionService } from '../../services/test-session.service';
import { ToastService } from '@viet-geeks/shared';
import { TestRunnerBaseComponent } from '../base/test-runner-base.component';

@Component({
  selector: 'viet-geeks-test-question',
  templateUrl: './test-question.component.html',
  styleUrls: ['./test-question.component.scss']
})
export class TestQuestionComponent extends TestRunnerBaseComponent implements OnInit, OnDestroy  {

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
  answers?: Array<string> | null;
  questionCount = 0;
  endTime: Date = new Date();
  remainingTime: TimeSpan = {};
  canSkipQuestion = false;
  canFinish = false;
  private subscription?: Subscription;

  constructor(private _fb: FormBuilder) {
    super();
    this.answerForm = this._fb.group({});
  }

  get canGoNext() {
    return this.index <= this.questionCount - 1;
  }

  get canGoBack() {
    return this.index > 0;
  }

  ngOnInit() {
    this.sessionData = this._testSessionService.getSessionData();
    this.index = this.sessionData.questionIndex ?? 0;
    this.question = this.sessionData.activeQuestion;
    this.questionCount = this.sessionData.questionCount ?? 0;
    this.canSkipQuestion = this.sessionData.canSkipQuestion ?? false;
    this.initAnswerForm();
    this.initEndTime();
    this.subscription = interval(1000)
      .subscribe(() => this.getTimeDifference());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }

  async submitAndGoNext() {
    await this.triggerWithLoadingIndicator(async () => {
      if (this.question) {
        this.showLoadingIndicator();
        const output = await firstValueFrom(this.proctorService.submitAnswer({
          questionId: this.question.id!,
          answerIds: this.getAnswerIds()
        }));

        if (output?.terminated) {
          this.hideLoadingIndicator();
          return this.finishExam();
        }
        else {
          return this.goToNextQuestion();
        }
      }
    });
  }

  async submitAndGoBack() {
    await this.triggerWithLoadingIndicator(async () => {
      if (this.question) {
        await firstValueFrom(this.proctorService.submitAnswer({
          questionId: this.question.id!,
          answerIds: this.getAnswerIds()
        }));

        this.goToPreviousQuestion();
      }
    });
  }

  private async goToPreviousQuestion() {
    const activateQuestionOutput = await firstValueFrom(this.proctorService.activePreviousQuestion());

    this.handleActivatedQuestion(activateQuestionOutput);
  }

  private async goToNextQuestion() {
    const activateQuestionOutput = await firstValueFrom(this.proctorService.activeNextQuestion());

    this.handleActivatedQuestion(activateQuestionOutput);
  }

  private handleActivatedQuestion(activateQuestionOutput: ActivateQuestionOutput | null) {
    if (!activateQuestionOutput?.activationResult) {
      this.finishExam();
    }
    else {
      this._testSessionService.setSessionData({
        examStep: ExamCurrentStep.SubmitAnswer,
        activeQuestion: activateQuestionOutput.activeQuestion,
        activeQuestionStartAt: null,
        activeQuestionAnswers: activateQuestionOutput.answerIds,
        questionIndex: activateQuestionOutput.activeQuestionIndex ?? undefined
      });

      this.question = activateQuestionOutput.activeQuestion;
      this.index = activateQuestionOutput.activeQuestionIndex ?? this.index;
      this.answers = activateQuestionOutput.answerIds;
      this.canFinish = activateQuestionOutput.canFinish ?? false;

      if (this.question) {
        this.initAnswerForm();
        this.initEndTime();
      }
      else if (!activateQuestionOutput.activationResult) {
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
        selectedAnswer: this.answers ?? []
      });
    }
    else {
      const selectedAnswerForms = this._fb.array([]) as FormArray;

      this.question!.answers?.forEach(answer => {
        const formGroup = this._fb.group({
          id: answer.id,
          selected: this.answers?.includes(answer.id!) ? true : false
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
    this._notifyService.warning('Time is up. The answer will be submitted automatically.');

    if (this.sessionData?.timeSettings?.method == TestDurationMethod.CompleteTestTime) {
      await this.finishExam();
    } else {
      await this.goToNextQuestion();
    }
  }

  private async finishExam() {
    const finishOutput = await firstValueFrom(this.proctorService.finishExam());
    this._testSessionService.setSessionData({
      endTime: finishOutput?.finishedAt ? new Date(finishOutput?.finishedAt) : new Date(),
      examStep: ExamCurrentStep.FinishExam,
      grading: finishOutput?.grading,
      questions: finishOutput?.questions,
      allAnswers: finishOutput?.examAnswers,
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
