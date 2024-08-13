import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestAccessComponent } from './pages/test-access/test-access.component';
import { TestFinishComponent } from './pages/test-finish/test-finish.component';
import { TestQuestionComponent } from './pages/test-question/test-question.component';
import { TestStartComponent } from './pages/test-start/test-start.component';
import { InTestSessionGuard } from './pages/common/guard/in-test-session.guard';
import { PublicTestAccessComponent } from './pages/public-test/public-test-access.component';

const routes: Routes = [
  {
    path: 'start',
    component: TestStartComponent,
    canActivate: [InTestSessionGuard]
  },
  {
    path: 'question',
    component: TestQuestionComponent,
    canActivate: [InTestSessionGuard]
  },
  {
    path: 'access',
    component: TestAccessComponent,
    canActivate: [InTestSessionGuard]
  },
  {
    path: 'access/:access-code',
    component: TestAccessComponent,
    canActivate: [InTestSessionGuard]
  },
  {
    path: 'finish',
    component: TestFinishComponent,
    canActivate: [InTestSessionGuard]
  },
  {
    path: 'public/:test-id',
    component: PublicTestAccessComponent,
    canActivate: []
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProctorRoutingModule { }
