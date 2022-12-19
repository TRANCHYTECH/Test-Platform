import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TestConfigurationRoutingModule } from './test-configuration-routing.module';
import { TestListComponent } from './pages/test-list/test-list.component';
import { TestCategoriesComponent } from './pages/test-categories/test-categories.component';
import { BasicSettingsComponent } from './pages/test-specific/basic-settings/basic-settings.component';
import { ManageQuestionsComponent } from './pages/test-specific/manage-questions/manage-questions.component';
import { EditQuestionComponent } from './pages/test-specific/edit-question/edit-question.component';
import { TestSetsComponent } from './pages/test-specific/test-sets/test-sets.component';
import { TestAccessComponent } from './pages/test-specific/test-access/test-access.component';
import { TestStartPageComponent } from './pages/test-specific/test-start-page/test-start-page.component';
import { GradingAndSummaryComponent } from './pages/test-specific/grading-and-summary/grading-and-summary.component';
import { TestTimeSettingsComponent } from './pages/test-specific/test-time-settings/test-time-settings.component';
import { TestSpecificLayoutComponent } from './layout/test-specific-layout/test-specific-layout.component';

@NgModule({
  declarations: [
    TestListComponent,
    TestCategoriesComponent,
    BasicSettingsComponent,
    ManageQuestionsComponent,
    EditQuestionComponent,
    TestSetsComponent,
    TestAccessComponent,
    TestStartPageComponent,
    GradingAndSummaryComponent,
    TestTimeSettingsComponent,
    TestSpecificLayoutComponent,
  ],
  imports: [CommonModule, TestConfigurationRoutingModule],
})
export class TestConfigurationModule {}
