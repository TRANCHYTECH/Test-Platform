import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuestionCategoryListComponent } from './question-category-list/question-category-list.component';
import { QuestionDetailsComponent } from './question-details/question-details.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { QuestionOrdersComponent } from './question-orders/question-orders.component';

const routes: Routes = [
  {
    path: 'list',
    title: 'Questions',
    component: QuestionListComponent
  },
  {
    path: 'order',
    title: 'Question Orders',
    component: QuestionOrdersComponent
  },
  {
    path: 'categories',
    title: 'Question Categories',
    component: QuestionCategoryListComponent
  },
  {
    path: ':question-id',
    title: 'Question',
    component: QuestionDetailsComponent
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
