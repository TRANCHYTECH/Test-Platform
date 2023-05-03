import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TestCategory, TestCategoryQuery, TestCategoryService } from '@viet-geeks/test-manager/state';
import { NgxSpinnerService } from 'ngx-spinner';
import { BehaviorSubject } from 'rxjs';
import { TestOverview } from '../../_state/test-overview.model';
import { TestOverviewService } from '../../_state/test-overview.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent implements OnInit, AfterViewInit {
  tests: TestOverview[] = [];
  testCategories: TestCategory[] = [];
  page = 1;
  pageSize = 12;

  private _readyForUI = new BehaviorSubject(false);
  private _spinner = inject(NgxSpinnerService);
  private _testOverviewService = inject(TestOverviewService);
  private _testCategoryQuery = inject(TestCategoryQuery);
  private _testCategoryService = inject(TestCategoryService);

  ngOnInit() {
    Promise.all([this._testOverviewService.get(), this._testCategoryService.get()]).then((rs) => {
      this.tests = rs[0];
      this.testCategories = this._testCategoryQuery.getAll();
      this._readyForUI.next(true);
    });

    this.configureLoadingIndicator();
  }

  ngAfterViewInit(): void {
    document.documentElement.setAttribute('data-sidebar-size', 'md');
  }

  showCategory(id: string) {
    return this._testCategoryQuery.getEntityWithFallback(id)?.name;
  }

  private configureLoadingIndicator() {
    this._readyForUI.pipe(untilDestroyed(this)).subscribe(v => {
      if (v === false) {
        this._spinner.show(undefined, {
          type: 'ball-fussion',
          size: 'medium',
          bdColor: 'rgba(100,149,237, .2)',
          color: 'white',
          fullScreen: false
        });
      } else {
        this._spinner.hide();
      }
    });
  }
}
