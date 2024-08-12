import { NgModule, inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterModule, Routes } from '@angular/router';
import { canDeactivateForm } from '@viet-geeks/shared';
import { BasicSettingsComponent } from './basic-settings/basic-settings.component';
import { GradingAndSummaryComponent } from './grading-and-summary/grading-and-summary.component';
import { OverviewComponent } from './overview/overview.component';
import { TestAccessComponent } from './test-access/test-access.component';
import { TestSetsComponent } from './test-sets/test-sets.component';
import { TestStartPageComponent } from './test-start-page/test-start-page.component';
import { TestTimeSettingsComponent } from './test-time-settings/test-time-settings.component';
import { TranslateService } from '@ngx-translate/core';

export const TestConfigRoutes = {
  BasicSettings: 'basic-settings',
  ManageQuestions: 'question',
  TestSets: 'test-sets',
  GradingAndSummary: 'grading-and-summary',
  TestAccess: 'test-access',
  TimeSettings: 'time-settings',
};

const getTestSettingsPath = (route: string) => {
  return `${route}`;
};

const routes: Routes = [
  {
    path: 'overview',
    component: OverviewComponent,
    title: 'Test Info',
    resolve: {
      isNewTest: isNewTest(),
    },
  },
  {
    path: getTestSettingsPath(TestConfigRoutes.BasicSettings),
    component: BasicSettingsComponent,
    title: () => inject(TranslateService).instant('testParts.basicSettings'),
    resolve: {
      isNewTest: isNewTest(),
    },
    canDeactivate: [canDeactivateForm],
  },
  {
    path: getTestSettingsPath(TestConfigRoutes.TestSets),
    component: TestSetsComponent,
    title: () => inject(TranslateService).instant('testParts.testSets'),
    canActivate: [isExistingTest()],
    canDeactivate: [canDeactivateForm],
  },
  {
    path: getTestSettingsPath(TestConfigRoutes.TestAccess),
    component: TestAccessComponent,
    title: () => inject(TranslateService).instant('testParts.testAccess'),
    canActivate: [isExistingTest()],
    canDeactivate: [canDeactivateForm],
  },
  {
    path: 'test-start-page',
    component: TestStartPageComponent,
    title: () => inject(TranslateService).instant('testParts.testStartPage'),
    canActivate: [isExistingTest()],
    canDeactivate: [canDeactivateForm],
  },
  {
    path: getTestSettingsPath(TestConfigRoutes.GradingAndSummary),
    component: GradingAndSummaryComponent,
    title: () => inject(TranslateService).instant('testParts.gradingAndSummary'),
    canActivate: [isExistingTest()],
    canDeactivate: [canDeactivateForm],
  },
  {
    path: getTestSettingsPath(TestConfigRoutes.TimeSettings),
    component: TestTimeSettingsComponent,
    title: () => inject(TranslateService).instant('testParts.timeSettings'),
    canActivate: [isExistingTest()],
    canDeactivate: [canDeactivateForm],
  },
  {
    path: getTestSettingsPath('question'),
    loadChildren: () =>
      import('../test-questions/questions.module').then(
        (m) => m.QuestionsModule
      ),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TestConfigurationRoutingModule {}

function isNewTest() {
  return (route: ActivatedRouteSnapshot) => route.params['id'] === 'new';
}

function isExistingTest() {
  //todo: check id is a valid format.
  return (route: ActivatedRouteSnapshot) => route.params['id'] !== 'new';
}
