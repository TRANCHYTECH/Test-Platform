import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { defaultPaginationConfig, PrimaryBaseComponent } from '@viet-geeks/shared';
import { TestCategory } from '../../../_state/test-category.model';
import { TestCategoryQuery } from '../../../_state/test-category.query';
import { TestCategoryService } from '../../../_state/test-category.service';
import { tap } from 'rxjs';
import { TestOverview } from '../../_state/test-overview.model';
import { TestOverviewService } from '../../_state/test-overview.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent extends PrimaryBaseComponent implements OnInit, AfterViewInit {
  tests: TestOverview[] = [];
  testCategories: TestCategory[] = [];

  readonly paginationConfig = Object.assign(defaultPaginationConfig(), { pageSizes: [4, 6, 12, 36, 72], pageSize: 12 });

  private _testOverviewService = inject(TestOverviewService);
  private _testCategoryQuery = inject(TestCategoryQuery);
  private _testCategoryService = inject(TestCategoryService);

  pagedSearchFn = (page: number, pageSize: number) => {
    this.maskBusyForMainFlow();

    return this._testOverviewService.get({ pageNumber: page, pageSize })
      .pipe(tap(rs => {
        this.tests = rs.results;

        this.maskReadyForMainFlow();
      }));
  };

  ngOnInit() {
    this.maskBusyForSupplyFlow();

    Promise.all([this._testCategoryService.get()]).then(() => {
      this.testCategories = this._testCategoryQuery.getAll();

      this.maskReadyForSupplyFlow();
    });

    this.configureLoadingIndicator();
  }

  ngAfterViewInit(): void {
    document.documentElement.setAttribute('data-sidebar-size', 'md');
  }

  showCategory(id: string) {
    return this._testCategoryQuery.getEntityWithFallback(id)?.name;
  }
}
