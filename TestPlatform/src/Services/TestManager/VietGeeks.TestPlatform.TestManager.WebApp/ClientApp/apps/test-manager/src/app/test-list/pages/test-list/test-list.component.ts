import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { NgxSpinnerService } from 'ngx-spinner';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { TestCategoriesQuery } from '../../../test-specific/_state/test-categories.query';
import { TestCategoriesService } from '../../../test-specific/_state/test-categories.service';
import { TestCategory } from '../../../test-specific/_state/test-category.model';
import { TestOverview } from '../../../test-specific/_state/test.model';
import { TestsService } from '../../../test-specific/_state/tests.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent implements OnInit, AfterViewInit{
  tests: TestOverview[] = [];
  testCategories: TestCategory[] = [];
  page = 1;
  pageSize = 12;

  private _readyForUI = new BehaviorSubject(false);
  private _spinner = inject(NgxSpinnerService);

  constructor(private _testsService: TestsService, private _testCategoriesQuery: TestCategoriesQuery,
    private _testCategoriesService: TestCategoriesService) {
  }

  ngOnInit() {
    Promise.all([firstValueFrom(this._testsService.getOverviews()), firstValueFrom(this._testCategoriesService.get())]).then((rs) => {
      this.tests = rs[0];
      this.testCategories = this._testCategoriesQuery.getAll();
      this._readyForUI.next(true);
    });

    this.configureLoadingIndicator();
  }

  ngAfterViewInit(): void {
    document.documentElement.setAttribute('data-sidebar-size', 'md');
  }

  showCategory(id: string) {
    return this.testCategories.find(c => c.id === id)?.name || 'Unknown';
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