import { ChangeDetectionStrategy, Component } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TestReportBaseComponent } from '../_components/test-report-base.component';
import { ExamSummary } from '../_state/exam-summary.model';

@Component({
  selector: 'viet-geeks-result-list',
  templateUrl: './result-list.component.html',
  styleUrls: ['./result-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ResultListComponent extends TestReportBaseComponent {
  examSummaries: ExamSummary[] = [];

  get currentUtcOffset() {
    return this._userProfileService.currentUtcOffset;
  }

  override async postLoadEntity(): Promise<void> {
    // Load test runs
    await super.postLoadEntity();

    // Load exam summaries of all test runs by default.
    await this.loadExamSummaries(this.testRuns.map(c => c.id));
    this.changeRef.markForCheck();
  }

  private async loadExamSummaries(testRunIds: string[]) {
    this.examSummaries = await firstValueFrom(this._examSummaryService.get(testRunIds));
  }

  async testRunsSelected(testRunIds: string[]) {
    await this.invokeLongAction(() => this.loadExamSummaries(testRunIds));
  }

  showDetails(exam: ExamSummary) {
    this.router.navigate(['../test-sheet-review'], { relativeTo: this.route, queryParams: {
      examId: exam.id
    } });
  }
}
