import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { CanComponentDeactivate, getTestId, IdService, TextEditorConfigsService, ToastService } from '@viet-geeks/shared';
import { isNumber } from 'lodash-es';
import { firstValueFrom, from, lastValueFrom, Observable, of } from 'rxjs';
import { QuestionCategory, QuestionCategoryGenericId } from '../../../_state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../_state/question-categories/question-categories.service';
import { Answer, AnswerType, Question, ScoreSettings } from '../../../_state/questions/question.model';
import { QuestionsQuery } from '../../../_state/questions/question.query';
import { QuestionService } from '../../../_state/questions/question.service';
import { CreateCategoryComponent } from '../../_components/create-test-category/create-test-category.component';

const AnswerTypes = [
  {
    id: AnswerType.SingleChoice,
    text: 'Single choice'
  },
  {
    id: AnswerType.MultipleChoice,
    text: 'Multiple choices'
  },
  {
    id: 3,
    text: 'Descriptive'
  },
  {
    id: 4,
    text: 'True/False'
  },
  {
    id: 5,
    text: 'Short Answer'
  }
];

@UntilDestroy()
@Component({
  selector: 'viet-geeks-question-details',
  templateUrl: './question-details.component.html',
  styleUrls: ['./question-details.component.scss']
})
export class QuestionDetailsComponent implements OnInit, CanComponentDeactivate {
  questionCategories$!: Observable<QuestionCategory[]>;
  questionForm: FormGroup;
  scoreSettingsForm: FormGroup;
  answerTypes = AnswerTypes;

  route = inject(ActivatedRoute);
  router = inject(Router);
  notifyService = inject(ToastService);
  textEditorConfigs = inject(TextEditorConfigsService);

  questionId: string;
  testId: string;
  isMultipleChoiceAnswer: boolean;
  answerType?: AnswerType;
  isPartialScore: boolean;
  singleChoiceIndex?: number;
  private isSubmitted = false;

  constructor(
    private _fb: FormBuilder,
    private _idService: IdService,
    private _questionCategoriesQuery: QuestionCategoriesQuery,
    private _questionCategoriesService: QuestionCategoriesService,
    private _questionService: QuestionService,
    private _questionQuery: QuestionsQuery,
    private _modalService: NgbModal
  ) {
    this.questionId = '';
    this.testId = '';
    this.isMultipleChoiceAnswer = false;
    this.isPartialScore = false;
    this.questionForm = this._fb.group({
      description: ['', [Validators.required]],
      categoryId: [QuestionCategoryGenericId, [Validators.required]],
      answerType: ['', [Validators.required]],
      answers: this._fb.array([])
    });

    const pointMandatoryCondition = () => (this.answerType == AnswerType.SingleChoice || this.answerType == AnswerType.MultipleChoice) && !this.isPartialScore;
    const partialPointMandatoryCondition =
      () => {
        return this.answerType === AnswerType.MultipleChoice && this.isPartialScore;
      }

    this.scoreSettingsForm = this._fb.group({
      correctPoint: [0, [RxwebValidators.compose({
        validators: [Validators.required, RxwebValidators.range({ minimumNumber: 0, maximumNumber: 1000 })], conditionalExpression: pointMandatoryCondition
      })]],
      incorrectPoint: [0, [RxwebValidators.compose({
        validators: [Validators.required, RxwebValidators.range({ minimumNumber: -999, maximumNumber: 0 })], conditionalExpression: pointMandatoryCondition
      })]],
      isPartialAnswersEnabled: false,
      bonusPoints: [0, [RxwebValidators.compose({
        validators: [Validators.required, RxwebValidators.range({ minimumNumber: 0, maximumNumber: 1000 })], conditionalExpression: partialPointMandatoryCondition
      })]],
      partialIncorrectPoint: [0, [RxwebValidators.compose({
        validators: [Validators.required, RxwebValidators.range({ minimumNumber: -999, maximumNumber: 0 })], conditionalExpression: partialPointMandatoryCondition
      })]],
      isDisplayMaximumScore: false,
      mustAnswerToContinue: false,
      isMandatory: false
    });
  }
  canDeactivate() {
    if (!this.canSubmit || this.isSubmitted) {
      return of(true);
    }

    return from(this.notifyService.confirm('You have unsave changed. Are you sure you want to leave?')
      .then((result) => {
        return result.isConfirmed;
      }));
  }

  ngOnInit(): void {
    firstValueFrom(this._questionCategoriesService.get());
    this.questionCategories$ = this._questionCategoriesQuery.selectAll();
    this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      this.testId = getTestId(this.route);
      this.questionId = p['question-id'];
      if (this.questionId !== 'new') {
        const question = this._questionQuery.getEntity(this.questionId);
        this.questionForm.reset({
          description: question?.description,
          categoryId: question?.categoryId,
          answerType: question?.answerType
        });

        if (question?.scoreSettings) {
          this.scoreSettingsForm.reset({
            correctPoint: question.scoreSettings.correctPoint,
            incorrectPoint: question.scoreSettings.incorrectPoint,
            isPartialAnswersEnabled: question.scoreSettings.isPartialAnswersEnabled,
            bonusPoints: question.scoreSettings.bonusPoints,
            partialIncorrectPoint: question.scoreSettings.partialIncorrectPoint,
            isDisplayMaximumScore: question.scoreSettings.isDisplayMaximumScore,
            mustAnswerToContinue: question.scoreSettings.mustAnswerToContinue,
            isMandatory: question.scoreSettings.isMandatory
          });
        }

        if (question?.answers) {
          this.singleChoiceIndex = question?.answers?.findIndex(a => a.isCorrect);
          question.answers.forEach((answer) => {
            this.addAnswer(answer);
          });
        }

        this.isMultipleChoiceAnswer = this.isMultipleChoice(question?.answerType);
        this.answerType = question?.answerType;
        this.isPartialScore = question?.scoreSettings?.isPartialAnswersEnabled ?? false;
      }
    });
    this.registerControlEvents();
  }

  addCategory() {
    const modalRef = this._modalService.open(CreateCategoryComponent, { size: 'md', centered: true });
    modalRef.result.then(async (formValue: Partial<QuestionCategory>) => {
      await firstValueFrom(this._questionCategoriesService.add(formValue));
    }, reason => {
      console.log(reason);
    })
  }

  addEmptyAnswer() {
    const emptyAnswer: Answer = {
      id: this._idService.generateId(),
      answerDescription: '',
      answerPoint: 0,
      isCorrect: false
    }

    this.addAnswer(emptyAnswer);
  }

  removeAnswer(index: number) {
    this.answers.removeAt(index);
  }

  selectSingleChoice(index: number) {
    this.singleChoiceIndex = index;
  }

  get answers() {
    return this.questionForm.get('answers') as FormArray;
  }

  get canSubmit(): boolean {
    return this.questionForm.dirty || this.scoreSettingsForm.dirty;
  }

  submitFunc = async () => {
    if (!this.canSubmit) {
      return;
    }

    await this.submit();
  };

  async submit() {
    try {
      if (this.questionForm.invalid || this.scoreSettingsForm.invalid) {
        return;
      }

      const formValue = this.questionForm.value;
      const answerType = parseInt(formValue.answerType);
      const question: Question = {
        ...formValue,
        answerType: answerType,
        questionNo: 1,//TODO
        scoreSettings: this.getScoreSettingsFormValue(answerType),
        id: this.questionId
      };

      if (!this.isMultipleChoice(question.answerType)) {
        question.answers?.forEach((a, idx) => a.isCorrect = idx === this.singleChoiceIndex);
      }

      if (this.questionId === 'new') {
        await lastValueFrom(this._questionService.add(this.testId, question));
        this.router.navigate(['../list'], { relativeTo: this.route });
        this.notifyService.success('Question created');
      } else {
        await this._questionService.update(this.testId, this.questionId, question);
        this.router.navigate(['../list'], { relativeTo: this.route });
        this.notifyService.success('Question updated');
      }

      this.isSubmitted = true;
    }
    catch (e) {
      this.notifyService.error('Error occured while saving question');
    }
  }

  private getScoreSettingsFormValue(answerType: number): ScoreSettings {
    const formValue: ScoreSettings = this.scoreSettingsForm.value;
    if (!(isNumber(formValue.correctPoint))) {
      formValue.correctPoint = undefined;
    }
    if (!(isNumber(formValue.incorrectPoint))) {
      formValue.incorrectPoint = undefined;
    }
    if (!(isNumber(formValue.bonusPoints))) {
      formValue.bonusPoints = undefined;
    }
    if (!(isNumber(formValue.partialIncorrectPoint))) {
      formValue.partialIncorrectPoint = undefined;
    }

    return {
      $type: answerType,
      ...formValue
    };
  }

  private registerControlEvents() {
    const answerTypeControl = this.questionForm.get('answerType') as FormControl;
    answerTypeControl.valueChanges.pipe(untilDestroyed(this)).subscribe(t => {
      this.isMultipleChoiceAnswer = this.isMultipleChoice(t);
      this.answerType = t;
    });

    const isPartialAnswersEnabledControl = this.scoreSettingsForm.get('isPartialAnswersEnabled') as FormControl;
    isPartialAnswersEnabledControl.valueChanges.pipe(untilDestroyed(this)).subscribe(v => {
      this.isPartialScore = v;
      this.scoreSettingsForm.get('correctPoint')?.updateValueAndValidity();
      this.scoreSettingsForm.get('incorrectPoint')?.updateValueAndValidity();
      this.scoreSettingsForm.get('bonusPoints')?.updateValueAndValidity();
      this.scoreSettingsForm.get('partialIncorrectPoint')?.updateValueAndValidity();
    });
  }

  private isMultipleChoice(answerType?: AnswerType): boolean {
    return answerType == AnswerType.MultipleChoice;
  }

  private addAnswer(answer: Answer) {
    const formGroup = this._fb.group({
      id: answer.id,
      answerDescription: [answer.answerDescription, [Validators.required]],
      answerPoint: [answer.answerPoint, [RxwebValidators.compose({
        validators: [Validators.required], conditionalExpression: () => (this.answerType == AnswerType.MultipleChoice)
      })]],
      isCorrect: answer.isCorrect,
      selectedIndex: this.singleChoiceIndex
    });
    this.answers.push(formGroup);

    return formGroup;
  }
}
