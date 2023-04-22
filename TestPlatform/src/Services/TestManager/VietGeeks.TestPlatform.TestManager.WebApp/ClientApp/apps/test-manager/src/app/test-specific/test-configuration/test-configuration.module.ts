import { NgModule } from '@angular/core';
import { TestConfigurationRoutingModule } from './test-configuration-routing.module';
import { CanDeactivateGuard, SharedModule } from '@viet-geeks/shared';
import { NgbAccordionModule, NgbAlertModule, NgbDropdownModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputMaskModule } from '@ngneat/input-mask';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { errorTailorImports } from '@ngneat/error-tailor';
import {ClipboardModule} from '@angular/cdk/clipboard';
import { BasicSettingsComponent } from './basic-settings/basic-settings.component';
import { EditQuestionComponent } from './edit-question/edit-question.component';
import { GradingAndSummaryComponent } from './grading-and-summary/grading-and-summary.component';
import { ManageQuestionsComponent } from './manage-questions/manage-questions.component';
import { OverviewComponent } from './overview/overview.component';
import { TestAccessComponent } from './test-access/test-access.component';
import { TestSetsComponent } from './test-sets/test-sets.component';
import { TestStartPageComponent } from './test-start-page/test-start-page.component';
import { TestTimeSettingsComponent } from './test-time-settings/test-time-settings.component';

@NgModule({
  declarations: [
    BasicSettingsComponent,
    ManageQuestionsComponent,
    EditQuestionComponent,
    TestSetsComponent,
    TestAccessComponent,
    TestStartPageComponent,
    GradingAndSummaryComponent,
    TestTimeSettingsComponent,
    OverviewComponent
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TestConfigurationRoutingModule,
    SharedModule,
    NgbDropdownModule,
    NgbNavModule,
    NgbAccordionModule,
    NgbAlertModule,
    InputMaskModule,
    NgbPaginationModule,
    errorTailorImports,
    ClipboardModule
  ],
  providers: [CanDeactivateGuard]
})
export class TestConfigurationModule { }
