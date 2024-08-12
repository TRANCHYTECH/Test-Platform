import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbAccordionModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ExamResultOverviewComponent, ExamResultTimerComponent, QuestionViewComponent, SharedModule } from '@viet-geeks/shared';
import { RespondentSelectorComponent } from './_components/respondent-selector/respondent-selector.component';
import { TestRunSelectorComponent } from './_components/test-run-selector/test-run-selector.component';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestReportRoutingModule } from './test-report-routing.module';
import { TestSheetReviewComponent } from './test-sheet-review/test-sheet-review.component';

@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent,
    TestRunSelectorComponent,
    TestSheetReviewComponent,
    RespondentSelectorComponent
  ],
  imports: [
    NgbDropdownModule,
    FormsModule,
    SharedModule,
    NgbAccordionModule,
    TestReportRoutingModule,
    QuestionViewComponent,
    ExamResultOverviewComponent,
    ExamResultTimerComponent
  ]
})
export class TestReportModule { }
