import { NgModule } from '@angular/core';

import { TestListRoutingModule } from './test-list-routing.module';
import { TestListComponent } from './pages/test-list/test-list.component';
import { SharedModule } from '@viet-geeks/shared';
import { NgbPagination } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [TestListComponent],
  imports: [
    SharedModule,
    NgbPagination,
    TestListRoutingModule
  ]
})
export class TestListModule {
 }
