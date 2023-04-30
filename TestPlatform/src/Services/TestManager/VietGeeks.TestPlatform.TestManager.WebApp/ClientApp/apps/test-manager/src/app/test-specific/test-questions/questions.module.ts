import { NgModule, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { NgbDropdownModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { errorTailorImports } from '@ngneat/error-tailor';
import { SharedModule } from '@viet-geeks/shared';
import { UiIntegrationService } from '../../_state/ui-integration.service';
import { QuestionCategoryListComponent } from './question-category-list/question-category-list.component';
import { QuestionDetailsComponent } from './question-details/question-details.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { QuestionsRoutingModule } from './questions-routing.module';
import { NewQuestionCategoryComponent } from './new-question-category/new-question-category.component';

@NgModule({
  declarations: [
    QuestionCategoryListComponent,
    QuestionDetailsComponent,
    QuestionListComponent
  ],
  imports: [
    SharedModule,
    NgbDropdownModule,
    ReactiveFormsModule,
    NgbPaginationModule,
    errorTailorImports,
    QuestionsRoutingModule
  ]
})
export class QuestionsModule {
  uiService = inject(UiIntegrationService);

  constructor() {
    this.uiService.registerModal(NewQuestionCategoryComponent, 'NewQuestionCategory');
  }
 }
