import { ChangeDetectionStrategy, Component, ViewChild } from '@angular/core';
import { TestReportBaseComponent } from '../_components/test-report-base.component';
import { ExamReview, Respondent, ScoresPerQuestionCatalog } from '../_state/exam-summary.model';
import { NgSelectComponent } from '@ng-select/ng-select';
import { BehaviorSubject, firstValueFrom } from 'rxjs';

@Component({
  selector: 'viet-geeks-test-sheet-review',
  templateUrl: './test-sheet-review.component.html',
  styleUrls: ['./test-sheet-review.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSheetReviewComponent extends TestReportBaseComponent {
  @ViewChild('respondentSelector')
  respondentSelector!: NgSelectComponent;

  respondents: Respondent[] = [];

  selectedRespondent?: Respondent;

  examReview$ = new BehaviorSubject<ExamReview>({ questions: [], answers: {}, firstName: '', lastName: '', scores: [], grading: [] });

  override async postLoadEntity(): Promise<void> {
    await super.postLoadEntity();

    await this.loadRespondents(this.testRuns.map(c => c.id));
    this.changeRef.markForCheck();
  }

  async testRunsSelected(testRunIds: string[]) {
    //todo: only filter client sides with all data there.
    await this.invokeLongAction(() => this.loadRespondents(testRunIds));
  }

  respondentSelected($event: Respondent) {
    firstValueFrom(this._examSummaryService.getExamReview($event.examId)).then(rs => {
      this.examReview$.next(rs);
      //todo: check why need this.
      this.changeRef.markForCheck();
    });
  }

  displayScorePercentage(score: ScoresPerQuestionCatalog) {
    return `${((score.actualPoints / score.totalPoints) * 100).toFixed()}%`;
  }

  private async loadRespondents(testRunIds: string[]) {
    this.respondents = await firstValueFrom(this._examSummaryService.getRespondents(testRunIds));
  }
}
