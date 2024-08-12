import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestSheetReviewComponent } from './test-sheet-review/test-sheet-review.component';

const routes: Routes = [
  {
    path: 'list',
    component: ResultListComponent,
    title: 'Results'
  },
  {
    path: 'respondent-monitor',
    component: RespondentMonitoringComponent
  },
  {
    path: 'statistics',
    component: StatisticsComponent
  },
  {
    path: 'test-sheet-review',
    title: 'Test sheet review',
    component: TestSheetReviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestReportRoutingModule { }
