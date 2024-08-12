import { NgModule, inject } from '@angular/core';
import { SharedModule } from '@viet-geeks/shared';
import { TestCategoryListComponent } from './test-category-list/test-category-list.component';
import { TetsCategoriesRoutingModule } from './test-categories-routing.module';
import { UiIntegrationService } from '../_state';
import { NewTestCategoryComponent } from './new-test-category/new-test-category.component';

@NgModule({
  declarations: [
    TestCategoryListComponent
  ],
  imports: [
    SharedModule,
    TetsCategoriesRoutingModule
  ]
})
export class TetsCategoriesModule { 
  uiService = inject(UiIntegrationService);

  constructor() {
    this.uiService.registerModal(NewTestCategoryComponent, 'NewTestCategory');
  }
}
