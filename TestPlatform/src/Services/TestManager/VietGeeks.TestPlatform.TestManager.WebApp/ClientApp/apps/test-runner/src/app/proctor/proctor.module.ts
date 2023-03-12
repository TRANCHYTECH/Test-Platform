import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProctorRoutingModule } from './proctor-routing.module';
import { TestStartComponent } from './pages/test-start/test-start.component';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [TestStartComponent, TestAccessComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ProctorRoutingModule],
})
export class ProctorModule { }
