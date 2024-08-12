import { NgModule } from '@angular/core';

import { AccountRoutingModule } from './account-routing.module';
import { GeneralInformationComponent } from './general-information/general-information.component';
import { RegionalSettingsComponent } from './regional-settings/regional-settings.component';
import { AccountLayoutComponent } from './_layouts/account-layout/account-layout.component';
import { SharedModule } from '@viet-geeks/shared';
import { NgbAccordionModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    GeneralInformationComponent,
    RegionalSettingsComponent,
    AccountLayoutComponent
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    AccountRoutingModule,
    NgbAccordionModule,
    NgbNavModule,
    NgSelectModule
  ]
})
export class AccountModule { }
