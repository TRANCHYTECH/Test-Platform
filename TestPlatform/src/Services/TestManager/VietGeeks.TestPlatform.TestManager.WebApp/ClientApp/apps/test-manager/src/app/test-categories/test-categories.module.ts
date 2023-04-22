import { NgModule } from '@angular/core';
import { SharedModule } from '@viet-geeks/shared';
import { TestCategoryListComponent } from './test-category-list/test-category-list.component';
import { TetsCategoriesRoutingModule } from './test-categories-routing.module';

@NgModule({
  declarations: [
    TestCategoryListComponent
  ],
  imports: [
    SharedModule,
    TetsCategoriesRoutingModule
  ]
})
export class TetsCategoriesModule { }
