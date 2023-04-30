import { NgModule } from '@angular/core';
import { ActivatedRouteSnapshot, RouterModule, Routes } from '@angular/router';
import { BasicSettingsComponent } from './basic-settings/basic-settings.component';
import { GradingAndSummaryComponent } from './grading-and-summary/grading-and-summary.component';
import { OverviewComponent } from './overview/overview.component';
import { TestAccessComponent } from './test-access/test-access.component';
import { TestSetsComponent } from './test-sets/test-sets.component';
import { TestStartPageComponent } from './test-start-page/test-start-page.component';
import { TestTimeSettingsComponent } from './test-time-settings/test-time-settings.component';

export const TestConfigRoutes = {
  BasicSettings: 'basic-settings',
  ManageQuestions: 'manage-questions',
  TestSets: 'test-sets',
  GradingAndSummary: 'grading-and-summary',
  TestAccess: 'test-access',
  TimeSettings: 'time-settings'
};

const getTestSettingsPath = (route: string) => {
  return `${route}`;
}

const routes: Routes = [
  {
    path: 'overview',
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
    path: 'test-start-page',
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
  },
    {
    path: getTestSettingsPath('question'),
    title: 'Question Categories',
    loadChildren: () => import('../test-questions/questions.module').then(m => m.QuestionsModule)
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
