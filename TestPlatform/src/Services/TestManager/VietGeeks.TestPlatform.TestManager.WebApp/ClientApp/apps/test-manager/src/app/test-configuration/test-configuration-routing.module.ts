import { NgModule } from '@angular/core';
import { ActivatedRouteSnapshot, RouterModule, Routes } from '@angular/router';
import { CanDeactivateGuard } from '@viet-geeks/shared';
import { TestSpecificLayoutComponent } from './layout/test-specific-layout/test-specific-layout.component';
import { TestListComponent } from './pages/test-list/test-list.component';
import { BasicSettingsComponent } from './pages/test-specific/basic-settings/basic-settings.component';
import { EditQuestionComponent } from './pages/test-specific/edit-question/edit-question.component';
import { GradingAndSummaryComponent } from './pages/test-specific/grading-and-summary/grading-and-summary.component';
import { ManageQuestionsComponent } from './pages/test-specific/manage-questions/manage-questions.component';
import { TestAccessComponent } from './pages/test-specific/test-access/test-access.component';
import { TestSetsComponent } from './pages/test-specific/test-sets/test-sets.component';
import { TestStartPageComponent } from './pages/test-specific/test-start-page/test-start-page.component';
import { TestTimeSettingsComponent } from './pages/test-specific/test-time-settings/test-time-settings.component';
import { OverviewComponent } from './pages/test-specific/overview/overview.component';

export const TestConfigRoutes = {
  BasicSettings: 'basic-settings',
  ManageQuestions: 'manage-questions',
  TestSets: 'test-sets',
  GradingAndSummary: 'grading-and-summary',
  TestAccess: 'test-access',
  TimeSettings: 'time-settings'
};

const getTestSettingsPath = (route: string) => {
  return `:id/${route}`;
}

const routes: Routes = [
  {
    path: '',
    component: TestListComponent,
    title: 'My Tests',
    pathMatch: 'full'
  },
  {
    path: '',
    component: TestSpecificLayoutComponent,
    children: [
      {
        path: ':id',
        component: OverviewComponent,
        title: 'Test Info',
        resolve: {
          isNewTest: isNewTest()
        }
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.BasicSettings),
        component: BasicSettingsComponent,
        title: 'Basic Settings',
        resolve: {
          isNewTest: isNewTest()
        }
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.ManageQuestions),
        component: ManageQuestionsComponent,
        title: 'Manage Questions',
        canActivate: [isExistingTest()]
      },
      {
        path: ':id/manage-questions/:question-id',
        component: EditQuestionComponent,
        title: 'Edit Question',
        canActivate: [isExistingTest()],
        canDeactivate: [CanDeactivateGuard]
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.TestSets),
        component: TestSetsComponent,
        title: 'Test Sets',
        canActivate: [isExistingTest()]
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.TestAccess),
        component: TestAccessComponent,
        title: 'Test Access',
        canActivate: [isExistingTest()]
      },
      {
        path: ':id/test-start-page',
        component: TestStartPageComponent,
        title: 'Test Start Page',
        canActivate: [isExistingTest()]
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.GradingAndSummary),
        component: GradingAndSummaryComponent,
        title: 'Grading & Summary',
        canActivate: [isExistingTest()]
      },
      {
        path: getTestSettingsPath(TestConfigRoutes.TimeSettings),
        component: TestTimeSettingsComponent,
        title: 'Time Settings',
        canActivate: [isExistingTest()]
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

function isExistingTest() {
  //todo: check id is a valid format.
  return (route: ActivatedRouteSnapshot) => route.params['id'] !== 'new';
}
