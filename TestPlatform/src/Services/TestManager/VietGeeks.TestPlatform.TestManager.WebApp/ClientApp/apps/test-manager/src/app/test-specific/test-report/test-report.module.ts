import { NgModule } from '@angular/core';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestReportRoutingModule } from './test-report-routing.module';
import { SharedModule } from '@viet-geeks/shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { TestRunSelectorComponent } from './_components/test-run-selector/test-run-selector.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent,
    TestRunSelectorComponent
  ],
  imports: [
    NgbDropdownModule,
    FormsModule,
    SharedModule,
    TestReportRoutingModule
  ]
})
export class TestReportModule { }
