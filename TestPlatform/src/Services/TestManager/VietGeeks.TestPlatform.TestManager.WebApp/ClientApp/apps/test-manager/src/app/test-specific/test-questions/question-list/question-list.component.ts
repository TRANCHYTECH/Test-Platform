import { ChangeDetectionStrategy, Component, ViewChild, inject } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { BehaviorSubject, firstValueFrom, tap } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionCategoriesQuery } from '../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';
import { AnswerType, Question } from '../../_state/questions/question.model';
import { QuestionsQuery } from '../../_state/questions/question.query';
import { QuestionService } from '../../_state/questions/question.service';
import { PaginationComponent } from '@viet-geeks/shared';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-question-list',
  templateUrl: './question-list.component.html',
  styleUrls: ['./question-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionListComponent extends TestSpecificBaseComponent {
  @ViewChild('pagination', {static: true})
  paginationComp!: PaginationComponent;

  AnswerType = AnswerType;
  questions$ = new BehaviorSubject<Question[]>([]);

  private _questionsQuery = inject(QuestionsQuery);
  private _questionsService = inject(QuestionService);
  private _questionCategoriesQuery = inject(QuestionCategoriesQuery);
  private _questionCategoriesService = inject(QuestionCategoriesService);

  pagedSearchFn = (page: number, pageSize: number) => {
    this.maskBusyForMainFlow();

    return this._questionsService.get(this.testId, { pageNumber: page, pageSize })
      .pipe(tap(() => this.maskReadyForMainFlow()));
  }

  async postLoadEntity(): Promise<void> {
    this.maskBusyForSupplyFlow();

    await Promise.all([firstValueFrom(this._questionCategoriesService.get(this.testId))]);

    this._questionsQuery.selectAll().pipe(untilDestroyed(this)).subscribe(questions => {
      this.questions$.next(questions);
    });

    this.maskReadyForSupplyFlow();
  }

  displayCategory(id: string) {
    return this._questionCategoriesQuery.getEntityWithFallback(id)?.name;
  }

  removeQuestion(questionId: string) {
    this._questionsService.remove(this.testId, questionId).then(() => this.paginationComp.refresh());
  }

  goToQuestionDetails(questionId: string, e: Event) {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const fromQuestionActionMenu = e.composedPath().find((c: any) => (typeof c.className) === "string" && c.className.includes('question-action-menu'));
    if (fromQuestionActionMenu === undefined)
      this.router.navigate(['../', questionId], { relativeTo: this.route });
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }
}
