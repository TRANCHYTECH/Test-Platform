import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';

const routes: Routes = [{
  path: 'start',
  component: TestStartComponent
}, {
  path: 'question',
  component: TestQuestionComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProctorRoutingModule { }
