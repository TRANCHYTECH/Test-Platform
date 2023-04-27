import { NgModule } from '@angular/core';

import { QuestionsRoutingModule } from './questions-routing.module';
import { QuestionCategoryListComponent } from './question-category-list/question-category-list.component';
import { NewQuestionCategoryComponent } from './new-question-category/new-question-category.component';
import { QuestionDetailsComponent } from './question-details/question-details.component';
import { QuestionCategoriesComponent } from './question-categories/question-categories.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { NgbDropdownModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '@viet-geeks/shared';
import { ReactiveFormsModule } from '@angular/forms';
import { errorTailorImports } from '@ngneat/error-tailor';


@NgModule({
  declarations: [
    QuestionCategoryListComponent,
    NewQuestionCategoryComponent,
    QuestionDetailsComponent,
    QuestionCategoriesComponent,
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
export class QuestionsModule { }
