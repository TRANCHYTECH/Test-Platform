import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestSpecificLayoutComponent } from './layout/test-specific-layout/test-specific-layout.component';

const routes: Routes = [
  {
    path: '',
    component: TestSpecificLayoutComponent,
    children: [
      {
        path: 'config',
        loadChildren: () => import('./test-configuration/test-configuration.module').then(m => m.TestConfigurationModule)
      },
      {
        path: 'report',
        loadChildren: () => import('./test-report/test-report.module').then(m => m.TestReportModule)
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class TestSpecificRoutingModule { }
