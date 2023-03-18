import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';

const routes: Routes = [
  {
  path: 'start',
  component: TestStartComponent
}, {
  path: 'question',
  component: TestQuestionComponent
},
{
  path: 'access',
  component: TestAccessComponent
},
{
  path: 'access/:access-code',
  component: TestAccessComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProctorRoutingModule { }
