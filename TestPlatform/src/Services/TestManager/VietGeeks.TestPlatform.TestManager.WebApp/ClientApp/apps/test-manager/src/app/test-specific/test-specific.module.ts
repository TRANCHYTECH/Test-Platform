import { NgModule } from '@angular/core';
import { TestSpecificRoutingModule } from './test-specific-routing.module';
import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '@viet-geeks/shared';
import { TestSpecificLayoutComponent } from './layout/test-specific-layout/test-specific-layout.component';

@NgModule({
  declarations: [
    TestSpecificLayoutComponent
  ],
  imports: [
    TestSpecificRoutingModule,
    SharedModule,
    NgbAccordionModule
  ]
})
export class TestSpecificModule { }
