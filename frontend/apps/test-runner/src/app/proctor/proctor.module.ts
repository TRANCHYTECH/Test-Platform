import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ExamResultOverviewComponent, ExamResultTimerComponent, FormatTimespanPipe } from '@viet-geeks/shared';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { TestFinishAnswersComponent } from './pages/test-finish/test-finish-answers/test-finish-answers.component';
import { TestFinishComponent } from './pages/test-finish/test-finish.component';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';
import { ProctorRoutingModule } from './proctor-routing.module';
import { errorTailorImports } from '@ngneat/error-tailor';

@NgModule({
  declarations: [TestStartComponent, TestAccessComponent, TestQuestionComponent, TestFinishComponent, TestFinishAnswersComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, errorTailorImports, ProctorRoutingModule, ExamResultOverviewComponent, ExamResultTimerComponent, FormatTimespanPipe],
  providers: [],
})
export class ProctorModule { }
