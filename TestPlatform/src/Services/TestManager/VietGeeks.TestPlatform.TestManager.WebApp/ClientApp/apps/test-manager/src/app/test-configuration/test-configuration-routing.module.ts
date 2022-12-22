import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestSpecificLayoutComponent } from './layout/test-specific-layout/test-specific-layout.component';
import { TestListComponent } from './pages/test-list/test-list.component';
import { BasicSettingsComponent } from './pages/test-specific/basic-settings/basic-settings.component';
import { GradingAndSummaryComponent } from './pages/test-specific/grading-and-summary/grading-and-summary.component';
import { ManageQuestionsComponent } from './pages/test-specific/manage-questions/manage-questions.component';
import { TestAccessComponent } from './pages/test-specific/test-access/test-access.component';
import { TestSetsComponent } from './pages/test-specific/test-sets/test-sets.component';
import { TestStartPageComponent } from './pages/test-specific/test-start-page/test-start-page.component';
import { TestTimeSettingsComponent } from './pages/test-specific/test-time-settings/test-time-settings.component';

const routes: Routes = [
  {
    path: '',
    component: TestListComponent
  },
  {
    path: ':testId',
    component: TestSpecificLayoutComponent,
    children: [
      {
        path: 'basic-settings',
        component: BasicSettingsComponent
      },
      {
        path: 'manage-questions',
        component: ManageQuestionsComponent
      },
      {
        path: 'test-sets',
        component: TestSetsComponent
      },
      {
        path: 'time-settings',
        component: TestTimeSettingsComponent
      },
      {
        path: 'test-access',
        component: TestAccessComponent
      },
      {
        path: 'test-start-page',
        component: TestStartPageComponent
      },
      {
        path: 'grading-and-summary',
        component: GradingAndSummaryComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestConfigurationRoutingModule { }
