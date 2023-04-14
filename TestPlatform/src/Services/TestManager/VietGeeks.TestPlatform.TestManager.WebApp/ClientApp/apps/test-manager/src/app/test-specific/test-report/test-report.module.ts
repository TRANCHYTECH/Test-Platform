import { NgModule } from '@angular/core';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestReportRoutingModule } from './test-report-routing.module';
import { SharedModule } from '@viet-geeks/shared';

@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent
  ],
  imports: [
    SharedModule,
    TestReportRoutingModule
  ]
})
export class TestReportModule { }
