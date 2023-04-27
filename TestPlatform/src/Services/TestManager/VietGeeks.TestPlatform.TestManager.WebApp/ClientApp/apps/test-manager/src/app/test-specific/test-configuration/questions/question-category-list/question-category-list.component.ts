
import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { UiIntegrationService } from '@viet-geeks/test-manager/state';
import { Subject } from 'rxjs';
import { QuestionCategory, QuestionCategoryGenericId } from '../../../_state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../_state/question-categories/question-categories.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-question-category-list',
  templateUrl: './question-category-list.component.html',
  styleUrls: ['./question-category-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionCategoryListComponent implements OnInit {
  categories$ = new Subject<QuestionCategory[]>;

  private _testCategoryService = inject(QuestionCategoriesService);
  private _uiIntegrationService = inject(UiIntegrationService);
  private _testCategoryQuery = inject(QuestionCategoriesQuery);

  ngOnInit(): void {
    this.loadData();
  }

  removeCategory(id: string) {
    this._testCategoryService.remove(id);
  }

  openNewTestCategoryModal() {
    this._uiIntegrationService.openNewTestCategoryModal();
  }

  private async loadData() {
    await this._testCategoryService.get();

    this._testCategoryQuery
      .selectAll({ filterBy: entity => entity.id !== QuestionCategoryGenericId })
      .pipe(untilDestroyed(this))
      .subscribe(rs => this.categories$.next(rs));
  }
}
