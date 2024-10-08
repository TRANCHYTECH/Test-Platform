import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountLayoutComponent } from './_layouts/account-layout/account-layout.component';
import { GeneralInformationComponent } from './general-information/general-information.component';
import { RegionalSettingsComponent } from './regional-settings/regional-settings.component';

const routes: Routes = [
  {
    path: '',
    component: AccountLayoutComponent,
    children: [
      {
        path: 'general-information',
        component: GeneralInformationComponent,
        title: 'General Information'
      },
      {
        path: 'regional-settings',
        component: RegionalSettingsComponent,
        title: 'Regional Settings'
      },
      {
        path: '**',
        redirectTo: 'general-information',
        pathMatch: 'full'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
