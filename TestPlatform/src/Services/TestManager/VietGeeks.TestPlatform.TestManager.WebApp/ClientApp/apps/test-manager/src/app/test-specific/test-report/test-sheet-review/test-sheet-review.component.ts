import { Component, ViewChild } from '@angular/core';
import { TestReportBaseComponent } from '../_components/test-report-base.component';
import { Respondent } from '../_state/exam-summary.model';
import { NgSelectComponent } from '@ng-select/ng-select';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'viet-geeks-test-sheet-review',
  templateUrl: './test-sheet-review.component.html',
  styleUrls: ['./test-sheet-review.component.scss']
})
export class TestSheetReviewComponent extends TestReportBaseComponent {
  @ViewChild('respondentSelector')
  respondentSelector!: NgSelectComponent;

  respondents: Respondent[] = [];

  selectedRespondent?: Respondent;

  override async postLoadEntity(): Promise<void> {
    await super.postLoadEntity();

    await this.loadRespondents(this.testRuns.map(c => c.id));
  }

  search(event: Event) {
    const term = (event.target as HTMLInputElement).value;
    this.respondentSelector.filter(term);
  }

  async testRunsSelected(testRunIds: string[]) {
    //todo: only filter client sides with all data there.
    await this.invokeLongAction(() => this.loadRespondents(testRunIds));
  }

  private async loadRespondents(testRunIds: string[]) {
    this.respondents = await firstValueFrom(this._examSummaryService.getRespondents(testRunIds));
  }
}
