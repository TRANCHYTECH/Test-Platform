import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgApexchartsModule } from 'ng-apexcharts';
import { ProctorRoutingModule } from './proctor-routing.module';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';
import { TestFinishComponent } from './pages/test-finish/test-finish.component';
import { FormatLocalDateTimePipe } from './pipes/format-local-datetime.pipe';
import { FormatTimespanPipe } from './pipes/format-timespan.pipe';


@NgModule({
  declarations: [TestStartComponent, TestAccessComponent, TestQuestionComponent, TestFinishComponent, FormatLocalDateTimePipe, FormatTimespanPipe],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ProctorRoutingModule, NgApexchartsModule],
  providers: [],
})
export class ProctorModule { }
