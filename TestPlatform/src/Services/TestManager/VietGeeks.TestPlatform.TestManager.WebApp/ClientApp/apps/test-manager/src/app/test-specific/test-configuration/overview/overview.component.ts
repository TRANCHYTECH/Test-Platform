import { Component, inject } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';

import { Summary, SummaryBuilder } from './summary-builder';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionService } from '../../_state/questions/question.service';
import { TestCategoryQuery } from '../../../_state/test-category.query';
import { TestCategoryService } from '../../../_state/test-category.service';
import { TestStatus } from '../../../_state/test-support.model';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent extends TestSpecificBaseComponent {
  summaries: Summary[] = [];
  private _testCategoryService = inject(TestCategoryService);
  private _testCategoryQuery = inject(TestCategoryQuery);
  private _questionService = inject(QuestionService);
  private _summaryBuilder = inject(SummaryBuilder);

  async postLoadEntity(): Promise<void> {
    const fetches = await Promise.all([this._questionService.getSummary(this.testId), this._testCategoryService.get()]);
    const questionSummary = fetches[0];
    const testCategories = this._testCategoryQuery.getAll();

    switch (this.test.status) {
      case TestStatus.Draft:
        this.summaries = this._summaryBuilder.buildSummaryForDaftTest(this.test, testCategories, questionSummary);
        break;
      case TestStatus.Activated:
        this.summaries = this._summaryBuilder.buildSummaryForActiveTest(this.test, questionSummary);
        break;
      case TestStatus.Scheduled:
        this.summaries = this._summaryBuilder.buildSummaryForScheduledTest(this.test);
        break;
      default:
        break;
    }
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

}
