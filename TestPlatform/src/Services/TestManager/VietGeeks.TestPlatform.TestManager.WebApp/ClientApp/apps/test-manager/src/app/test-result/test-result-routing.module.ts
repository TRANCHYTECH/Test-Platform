import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResultListComponent } from './pages/result-list/result-list.component';
import { RespondentMonitoringComponent } from './pages/respondent-monitoring/respondent-monitoring.component';
import { StatisticsComponent } from './pages/statistics/statistics.component';

const routes: Routes = [
  {
    path: 'list',
    component: ResultListComponent
  },
  {
    path: 'respondent-monitor',
    component: RespondentMonitoringComponent
  },
  {
    path: 'statistics',
    component: StatisticsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestResultRoutingModule { }
