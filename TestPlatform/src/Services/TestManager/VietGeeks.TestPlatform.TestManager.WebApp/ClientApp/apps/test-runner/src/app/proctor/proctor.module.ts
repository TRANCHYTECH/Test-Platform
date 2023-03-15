import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProctorRoutingModule } from './proctor-routing.module';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';

@NgModule({
  declarations: [TestStartComponent, TestAccessComponent, TestQuestionComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ProctorRoutingModule],
})
export class ProctorModule { }
