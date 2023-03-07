import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProctorRoutingModule } from './proctor-routing.module';
import { TestStartComponent } from './pages/test-start/test-start.component';

@NgModule({
  declarations: [TestStartComponent],
  imports: [CommonModule, ProctorRoutingModule],
})
export class ProctorModule {}
