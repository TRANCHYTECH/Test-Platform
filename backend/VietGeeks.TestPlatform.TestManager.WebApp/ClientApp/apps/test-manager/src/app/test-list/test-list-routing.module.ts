import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestListComponent } from './pages/test-list/test-list.component';

const routes: Routes = [
  {
    path: '',
    component: TestListComponent,
    title: 'My Tests',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestListRoutingModule { }
