
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, firstValueFrom } from 'rxjs';
import { UiIntegrationService } from '../../../_state/ui-integration.service';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionCategory, QuestionCategoryGenericId } from '../../_state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';

@Component({
  selector: 'viet-geeks-question-category-list',
  templateUrl: './question-category-list.component.html',
  styleUrls: ['./question-category-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionCategoryListComponent extends TestSpecificBaseComponent {
  categories$ = new Subject<QuestionCategory[]>;

  private _questionCategoryService = inject(QuestionCategoriesService);
  private _uiIntegrationService = inject(UiIntegrationService);
  private _questionCategoryQuery = inject(QuestionCategoriesQuery);

  override postLoadEntity(): void | Promise<void> {
    this.loadData();
  }

  override submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  override get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

  removeCategory(id: string) {
    this._questionCategoryService.remove(this.testId, id);
  }

  openNewTestCategoryModal() {
    this._uiIntegrationService.openModal('NewQuestionCategory', { testId: this.testId });
  }

  private async loadData() {
    await firstValueFrom(this._questionCategoryService.get(this.testId));

    this._questionCategoryQuery
      .selectAll({ filterBy: entity => entity.id !== QuestionCategoryGenericId })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe(rs => this.categories$.next(rs));
  }
}
