import { ChangeDetectionStrategy, Component, Input, ViewChild } from '@angular/core';
import { NgSelectComponent } from '@ng-select/ng-select';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { TestReportBaseComponent } from '../_components/test-report-base.component';
import { ExamReview, Respondent, ScoresPerQuestionCatalog } from '../_state/exam-summary.model';

@Component({
  selector: 'viet-geeks-test-sheet-review',
  templateUrl: './test-sheet-review.component.html',
  styleUrls: ['./test-sheet-review.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSheetReviewComponent extends TestReportBaseComponent {
  @ViewChild('respondentSelector')
  respondentSelector!: NgSelectComponent;

  @Input()
  examId?: string;

  respondentSelectorInput?: { dataSource: Respondent[], preSelectedExam?: string };

  selectedRespondent?: Respondent;

  examReview$ = new BehaviorSubject<Partial<ExamReview>>({ questions: [], answers: {}, scores: [], grading: [] });

  get examReview() {
    return this.examReview$.value as ExamReview;
  }

  override async postLoadEntity(): Promise<void> {
    await super.postLoadEntity();

    await this.loadRespondents(this.testRuns.map(c => c.id));
    this.changeRef.markForCheck();
  }

  override onInit(): void {
    console.log('on init', this.examId);
  }

  async testRunsSelected(testRunIds: string[]) {
    //todo: only filter client sides with all data there.
    await this.invokeLongAction(() => this.loadRespondents(testRunIds));
  }

  respondentSelected($event: Respondent) {
    this.maskBusyForMainFlow();
    firstValueFrom(this._examSummaryService.getExamReview($event.examId)).then(rs => {
      this.examReview$.next(rs);
      //todo: check why need this.
      this.changeRef.markForCheck();
    }).finally(() => this.maskReadyForMainFlow());
  }

  displayScorePercentage(score: ScoresPerQuestionCatalog) {
    return `${((score.actualPoints / score.totalPoints) * 100).toFixed()}%`;
  }

  private async loadRespondents(testRunIds: string[]) {
    const respondents = await firstValueFrom(this._examSummaryService.getRespondents(testRunIds));
    this.respondentSelectorInput = { dataSource: respondents, preSelectedExam: this.examId };
  }
}
