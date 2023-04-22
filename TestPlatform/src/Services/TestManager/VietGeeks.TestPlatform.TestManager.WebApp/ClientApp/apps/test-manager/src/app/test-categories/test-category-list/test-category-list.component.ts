import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TestCategory, TestCategoryQuery, TestCategoryService, TestCategoryUncategorizedId } from '@viet-geeks/test-manager/state';
import { Subject } from 'rxjs';
import { NewTestCategoryComponent } from '../new-test-category/new-test-category.component';

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
  private _testCategoryQuery = inject(TestCategoryQuery);
  private _modalService = inject(NgbModal);

  ngOnInit(): void {
    this.loadData();
  }

  removeCategory(id: string) {
    this._testCategoryService.remove(id);
  }

  openNewTestCategoryModal() {
    this._modalService.open(NewTestCategoryComponent, { size: 'md', centered: true });
  }

  private async loadData() {
    await this._testCategoryService.get();

    this._testCategoryQuery
      .selectAll({ filterBy: entity => entity.id !== TestCategoryUncategorizedId })
      .pipe(untilDestroyed(this))
      .subscribe(rs => this.categories$.next(rs));
  }
}
