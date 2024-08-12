import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestCategoryListComponent } from './test-category-list/test-category-list.component';

const routes: Routes = [
  {
    path: '',
    component: TestCategoryListComponent,
    title: 'Test categories'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TetsCategoriesRoutingModule { }
