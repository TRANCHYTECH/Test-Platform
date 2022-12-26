import { NgModule } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, RouterModule, RouterStateSnapshot, Routes } from '@angular/router';
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
    component: TestListComponent,
    pathMatch: 'full'
  },
  {
    path: '',
    component: TestSpecificLayoutComponent,
    children: [
      {
        path: ':id/basic-settings',
        component: BasicSettingsComponent,
        resolve: {
          isNewTest: isNewTest()
        }
      },
      {
        path: ':id/manage-questions',
        component: ManageQuestionsComponent //todo: add guard to prevent new test
      },
      {
        path: ':id/test-sets',
        component: TestSetsComponent
      },
      {
        path: ':id/time-settings',
        component: TestTimeSettingsComponent
      },
      {
        path: ':id/test-access',
        component: TestAccessComponent
      },
      {
        path: ':id/test-start-page',
        component: TestStartPageComponent
      },
      {
        path: ':id/grading-and-summary',
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

function isNewTest() {
  return (route: ActivatedRouteSnapshot) => route.params['id'] === 'new';
}

