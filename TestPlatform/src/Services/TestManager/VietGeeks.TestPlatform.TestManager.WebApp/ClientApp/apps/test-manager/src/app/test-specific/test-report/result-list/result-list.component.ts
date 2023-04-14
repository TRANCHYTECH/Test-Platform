import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { UserProfileService } from '@viet-geeks/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { ExamSummary, TestRunSummary } from '../_state/exam-summary.model';
import { ExamSummaryService } from '../_state/exam-summary.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-result-list',
  templateUrl: './result-list.component.html',
  styleUrls: ['./result-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ResultListComponent extends TestSpecificBaseComponent implements OnInit {
  examSummaries: ExamSummary[] = [];
  testRuns: TestRunSummary[] = [];

  private _examSummaryService = inject(ExamSummaryService);
  private _userProfileService = inject(UserProfileService);

  get currentUtcOffset() {
    return this._userProfileService.currentUtcOffset;
  }

  override async afterGetTest(): Promise<void> {
    this.testRuns = await firstValueFrom(this._examSummaryService.getTestRuns(this.testId));
    await this.loadExamSummaries(this.testRuns.map(c => c.id));
    this.maskReadyForUI();
    this.changeRef.markForCheck();
  }

  private async loadExamSummaries(testRunIds: string[]) {
    this.examSummaries = await firstValueFrom(this._examSummaryService.get(testRunIds));
  }

  async testRunsSelected(testRunIds: string[]) {
    await this.invokeLongAction(() => this.loadExamSummaries(testRunIds));
  }

  override submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  override get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }
}
