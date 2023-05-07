import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TestCategory, TestCategoryUncategorizedId } from '../../_state/test-category.model';
import { TestCategoryQuery } from '../../_state/test-category.query';
import { TestCategoryService } from '../../_state/test-category.service';
import { UiIntegrationService } from '../../_state/ui-integration.service';
import { Subject } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-category-list',
  templateUrl: './test-category-list.component.html',
  styleUrls: ['./test-category-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestCategoryListComponent implements OnInit {
  categories$ = new Subject<TestCategory[]>;

  private _testCategoryService = inject(TestCategoryService);
  private _uiIntegrationService = inject(UiIntegrationService);
  private _testCategoryQuery = inject(TestCategoryQuery);

  ngOnInit(): void {
    this.loadData();
  }

  removeCategory(id: string) {
    this._testCategoryService.remove(id);
  }

  openNewTestCategoryModal() {
    this._uiIntegrationService.openModal('NewTestCategory', {});
  }

  private async loadData() {
    await this._testCategoryService.get();

    this._testCategoryQuery
      .selectAll({ filterBy: entity => entity.id !== TestCategoryUncategorizedId })
      .pipe(untilDestroyed(this))
      .subscribe(rs => this.categories$.next(rs));
  }
}
