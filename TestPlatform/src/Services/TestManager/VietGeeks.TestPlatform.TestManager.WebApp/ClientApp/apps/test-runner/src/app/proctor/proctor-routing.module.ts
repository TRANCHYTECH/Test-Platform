import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestStartComponent } from './pages/test-start/test-start.component';

const routes: Routes = [{
  path: 'start',
  component: TestStartComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProctorRoutingModule { }
