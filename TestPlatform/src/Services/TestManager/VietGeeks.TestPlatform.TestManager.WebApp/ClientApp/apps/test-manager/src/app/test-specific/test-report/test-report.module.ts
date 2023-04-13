import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestReportRoutingModule } from './test-report-routing.module';

@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent
  ],
  imports: [
    CommonModule,
    TestReportRoutingModule
  ]
})
export class TestReportModule { }
