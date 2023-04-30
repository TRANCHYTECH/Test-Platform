import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Subject, firstValueFrom } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionCategoriesQuery } from '../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';
import { AnswerType, Question } from '../../_state/questions/question.model';
import { QuestionsQuery } from '../../_state/questions/question.query';
import { QuestionService } from '../../_state/questions/question.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-question-list',
  templateUrl: './question-list.component.html',
  styleUrls: ['./question-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionListComponent extends TestSpecificBaseComponent {
  AnswerType = AnswerType;
  questions$ = new Subject<Question[]>();
  page = 0;
  pageSize = 10;

  private _questionsQuery = inject(QuestionsQuery);
  private _questionsService = inject(QuestionService);
  private _questionCategoriesQuery = inject(QuestionCategoriesQuery);
  private _questionCategoriesService = inject(QuestionCategoriesService);

  async postLoadEntity(): Promise<void> {
    await Promise.all([firstValueFrom(this._questionCategoriesService.get(this.testId)), firstValueFrom(this._questionsService.get(this.testId))]);
    this.questions$.next(this._questionsQuery.getAll());
  }

  displayCategory(id: string) {
    return this._questionCategoriesQuery.getEntityWithFallback(id)?.name;
  }

  removeQuestion(questionId: string) {
    this._questionsService.remove(this.testId, questionId);
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

}
