import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { firstValueFrom, map, Observable } from 'rxjs';
import { QuestionCategory } from '../../../state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../state/question-categories/question-categories.service';
import { AnswerType, Question } from '../../../state/questions/question.model';
import { QuestionsQuery } from '../../../state/questions/question.query';
import { QuestionService } from '../../../state/questions/question.service';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-manage-questions',
  templateUrl: './manage-questions.component.html',
  styleUrls: ['./manage-questions.component.scss']
})
export class ManageQuestionsComponent extends TestSpecificBaseComponent {
  questions$!: Observable<Question[]>;
  questionCategories$!: Observable<QuestionCategory[]>;
  questionCategories: { [id: string]: string } = {};
  page = 0;
  pageSize = 10;

  constructor(private _questionsQuery: QuestionsQuery, private _questionsService: QuestionService,
    private _questionCategoriesQuery: QuestionCategoriesQuery,
    private _questionCategoriesService: QuestionCategoriesService) {
    super();
  }

  onInit(): void {
    //
  }

  async afterGetTest(): Promise<void> {
    firstValueFrom(this._questionCategoriesService.get());
    this.questions$ = this._questionsQuery.selectAll().pipe(map((questions) => (questions.map(q => ({ ...q, ['answerTypeName']: AnswerType[q.answerType] })))));
    this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      this.testId = p['id'];
      if (!this.isNewTest) {
        await firstValueFrom(this._questionsService.get(this.testId), { defaultValue: null });
      }
    });
    this.questionCategories = (await firstValueFrom(this._questionCategoriesQuery.selectAll())).reduce((result, item) => ({ ...result, [item.id]: item.name }), {});
    this.maskReadyForUI();
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

}
