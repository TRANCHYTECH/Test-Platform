import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RespondentMonitoringComponent } from './respondent-monitoring/respondent-monitoring.component';
import { ResultListComponent } from './result-list/result-list.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestReportRoutingModule } from './test-report-routing.module';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { TestRunSelectorComponent } from './_components/test-run-selector/test-run-selector.component';

@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent,
    TestRunSelectorComponent
  ],
  imports: [
    CommonModule,
    TestReportRoutingModule,
    FormsModule,
    NgbDropdownModule
  ]
})
export class TestReportModule { }
