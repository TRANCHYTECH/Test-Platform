import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { ExamSummaryService } from '../../state/exam-summary.service';
import { ExamSummary, TestRunSummary } from '../../state/exam-summary.model';
import { firstValueFrom } from 'rxjs';
import { UserProfileService } from '@viet-geeks/core';
import { ActivatedRoute } from '@angular/router';
import { UntilDestroy } from '@ngneat/until-destroy';
import { TestSpecificBaseComponent } from '../../../test-configuration/pages/test-specific/base/test-specific-base.component';

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
  private _route = inject(ActivatedRoute);
  private _changeRef = inject(ChangeDetectorRef);

  get currentUtcOffset() {
    return this._userProfileService.currentUtcOffset;
  }

  override async afterGetTest(): Promise<void> {
    const testId = this._route.snapshot.params['id'];
    this.testRuns = await firstValueFrom(this._examSummaryService.getTestRuns(testId));
    this.examSummaries = await firstValueFrom(this._examSummaryService.get(this.testRuns.map(c => c.id)));
    this._changeRef.markForCheck();

    this.maskReadyForUI();
  }

  override submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  override get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }
}
