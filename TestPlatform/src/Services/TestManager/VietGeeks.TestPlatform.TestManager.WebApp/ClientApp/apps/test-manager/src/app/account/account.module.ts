import { NgModule } from '@angular/core';

import { AccountRoutingModule } from './account-routing.module';
import { GeneralInformationComponent } from './general-information/general-information.component';
import { RegionalSettingsComponent } from './regional-settings/regional-settings.component';
import { AccountLayoutComponent } from './_layouts/account-layout/account-layout.component';
import { SharedModule } from '@viet-geeks/shared';
import { NgbAccordionModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [
    GeneralInformationComponent,
    RegionalSettingsComponent,
    AccountLayoutComponent
  ],
  imports: [
    SharedModule,
    AccountRoutingModule,
    NgbAccordionModule,
    NgbNavModule
  ]
})
export class AccountModule { }
