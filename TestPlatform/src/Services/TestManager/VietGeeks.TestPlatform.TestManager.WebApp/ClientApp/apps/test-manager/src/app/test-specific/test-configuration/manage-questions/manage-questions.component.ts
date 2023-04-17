import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { firstValueFrom, map, Observable } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionCategory } from '../../_state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';
import { Question, AnswerType } from '../../_state/questions/question.model';
import { QuestionsQuery } from '../../_state/questions/question.query';
import { QuestionService } from '../../_state/questions/question.service';
import { getTestId } from '@viet-geeks/shared';

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

  async postLoadEntity(): Promise<void> {
    firstValueFrom(this._questionCategoriesService.get());
    this.questions$ = this._questionsQuery.selectAll().pipe(map((questions) => (questions.map(q => ({ ...q, ['answerTypeName']: AnswerType[q.answerType] })))));
    this.route.params.pipe(untilDestroyed(this)).subscribe(async () => {
      this.testId = getTestId(this.route);
      if (!this.isNewTest) {
        await firstValueFrom(this._questionsService.get(this.testId), { defaultValue: null });
      }
    });
    this.questionCategories = (await firstValueFrom(this._questionCategoriesQuery.selectAll())).reduce((result, item) => ({ ...result, [item.id]: item.name }), {});
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

}
