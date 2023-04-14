import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { GeneralInformationComponent } from './general-information/general-information.component';
import { RegionalSettingsComponent } from './regional-settings/regional-settings.component';
import { AccountLayoutComponent } from './_layout/account-layout/account-layout.component';


@NgModule({
  declarations: [
    GeneralInformationComponent,
    RegionalSettingsComponent,
    AccountLayoutComponent
  ],
  imports: [
    CommonModule,
    AccountRoutingModule
  ]
})
export class AccountModule { }
