import { Component, inject } from '@angular/core';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';
import { UntilDestroy } from '@ngneat/until-destroy';
import { firstValueFrom } from 'rxjs';
import { TestCategoriesService } from '../../../state/test-categories.service';
import { TestCategoriesQuery } from '../../../state/test-categories.query';
import { QuestionService } from '../../../state/questions/question.service';
import { TestStatus } from '../../../state/test.model';
import { Summary, SummaryBuilder } from './summary-builder';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent extends TestSpecificBaseComponent {
  summaries: Summary[] = [];
  private _testCategoriesService = inject(TestCategoriesService);
  private _testCategoriesQuery = inject(TestCategoriesQuery);
  private _questionService = inject(QuestionService);
  private _summaryBuilder = inject(SummaryBuilder);

  async afterGetTest(): Promise<void> {
    const fetches = await Promise.all([this._questionService.getSummary(this.testId), firstValueFrom(this._testCategoriesService.get())]);
    const questionSummary = fetches[0];
    const testCategories = this._testCategoriesQuery.getAll();

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

    this.maskReadyForUI();
  }

  submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }

}
