import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ToastService } from '@viet-geeks/shared';
import { IdService } from '../../../../../app/common/services/id.service';
import { firstValueFrom, lastValueFrom, Observable } from 'rxjs';
import { CreateCategoryComponent } from '../../../components/create-test-category/create-test-category.component';
import { QuestionCategory } from '../../../state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../state/question-categories/question-categories.service';
import { Answer, AnswerType, Question } from '../../../state/questions/question.model';
import { QuestionsQuery } from '../../../state/questions/question.query';
import { QuestionService } from '../../../state/questions/question.service';
import { AnswerTypes } from './data';

interface QuestionForm {
  questionDescription: FormControl<string | null>;
  category: FormControl<string | null>;
  answerType: FormControl<string | null>;
}

@UntilDestroy()
@Component({
  selector: 'viet-geeks-edit-question',
  templateUrl: './edit-question.component.html',
  styleUrls: ['./edit-question.component.scss'],
})
export class EditQuestionComponent implements OnInit {
  Editor = ClassicEditor;
  questionCategories$!: Observable<QuestionCategory[]>;
  questionForm: FormGroup;
  answerTypes = AnswerTypes;

  route = inject(ActivatedRoute);
  router = inject(Router);
  notifyService = inject(ToastService);

  questionId: string;
  testId: string;
  isMultipleChoiceAnswer: boolean;
  private _singleChoiceIndex?: number;

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
    this.questionForm = this._fb.group({
      description: '',
      categoryId: '',
      answerType: 0,
      answers: this._fb.array([]),
      selectedAnswer: ''
    });
  }

  ngOnInit(): void {
    firstValueFrom(this._questionCategoriesService.get());
    this.questionCategories$ = this._questionCategoriesQuery.selectAll();
    this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      this.testId = p['id'];
      this.questionId = p['question-id'];
      if (this.questionId !== 'new') {
        const question =  this._questionQuery.getEntity(this.questionId);
        this.questionForm.reset({
          description: question?.description,
          categoryId: question?.categoryId,
          answerType: question?.answerType
        });

        if (question?.answers) {
          question.answers.forEach(answer => {
            this.addAnswer(answer);
          });
        }

        this.isMultipleChoiceAnswer = this.isMultipleChoice(question?.answerType);
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
    this._singleChoiceIndex = index;
  }

  get answers() {
    return this.questionForm.get('answers') as FormArray;
  }

  async submit() {
    if (this.questionForm.invalid) {
      return;
    }

    const formValue = this.questionForm.value;
    const question: Question = {
      ...formValue,
      answerType: parseInt(formValue.answerType),
      questionNo: 1,//TODO
      scoreSettings: {},
      id: this.questionId
    };

    if (!this.isMultipleChoice(question.answerType)) {
      question.answers?.forEach((a, idx) => a.isCorrect = idx === this._singleChoiceIndex);
    }

    if (this.questionId === 'new') {
      await lastValueFrom(this._questionService.add(this.testId, question));
      this.router.navigate(['tests', this.testId, 'manage-questions']);
      this.notifyService.show('Question created');
    } else {
        await this._questionService.update(this.testId, this.questionId, question);
        this.router.navigate(['tests', this.testId, 'manage-questions']);
        this.notifyService.show('Question updated');
    }
  }

  private registerControlEvents() {
    const answerTypeControl = this.questionForm.get('answerType') as FormControl;
    answerTypeControl.valueChanges.pipe(untilDestroyed(this)).subscribe(t => {
        this.isMultipleChoiceAnswer = this.isMultipleChoice(t);
    });
  }

  private isMultipleChoice(answerType?: AnswerType): boolean {
    return answerType == AnswerType.MultipleChoice;
  }

  private addAnswer(answer: Answer) {
    this.answers.push(this._fb.group({
      id: answer.id,
      answerDescription : answer.answerDescription,
      answerPoint: answer.answerPoint,
      isCorrect: answer.isCorrect
    }));
  }
}
