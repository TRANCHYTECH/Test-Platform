import { NgModule, inject } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { canDeactivateForm } from '@viet-geeks/shared';
import { QuestionCategoryListComponent } from './question-category-list/question-category-list.component';
import { QuestionDetailsComponent } from './question-details/question-details.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { QuestionOrdersComponent } from './question-orders/question-orders.component';
import { TranslateService } from '@ngx-translate/core';

const routes: Routes = [
  {
    path: 'list',
    title: () => inject(TranslateService).instant('testParts.questions'),
    component: QuestionListComponent
  },
  {
    path: 'order',
    title: () => inject(TranslateService).instant('testParts.questionOrders'),
    component: QuestionOrdersComponent
  },
  {
    path: 'categories',
    title: () => inject(TranslateService).instant('testParts.questionCategories'),
    component: QuestionCategoryListComponent
  },
  {
    path: ':question-id',
    title: () => inject(TranslateService).instant('testParts.question'),
    component: QuestionDetailsComponent,
    canDeactivate: [canDeactivateForm]
  },
  {
    path: '**',
    redirectTo: 'list'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QuestionsRoutingModule { }
