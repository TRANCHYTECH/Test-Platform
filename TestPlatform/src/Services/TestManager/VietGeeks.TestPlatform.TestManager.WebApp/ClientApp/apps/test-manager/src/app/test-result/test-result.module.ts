import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TestResultRoutingModule } from './test-result-routing.module';
import { ResultListComponent } from './pages/result-list/result-list.component';
import { RespondentMonitoringComponent } from './pages/respondent-monitoring/respondent-monitoring.component';
import { StatisticsComponent } from './pages/statistics/statistics.component';


@NgModule({
  declarations: [
    ResultListComponent,
    RespondentMonitoringComponent,
    StatisticsComponent
  ],
  imports: [
    CommonModule,
    TestResultRoutingModule
  ]
})
export class TestResultModule { }
