import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FeatherModule } from 'angular-feather';
import { allIcons } from 'angular-feather/icons';
import { BreadcrumbsComponent } from './components/breadcrumbs/breadcrumbs.component';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { NgxSpinnerModule } from "ngx-spinner";
import { SubmitButtonComponent } from './components/submit-button.component';

@NgModule({
  imports: [CommonModule, HttpClientModule, FeatherModule.pick(allIcons), NgbToastModule],
  declarations: [BreadcrumbsComponent, SubmitButtonComponent],
  exports: [
    HttpClientModule,
    TranslateModule,
    FeatherModule,
    BreadcrumbsComponent,
    SubmitButtonComponent,
    SweetAlert2Module,
    NgxSpinnerModule
  ]
})
export class SharedModule { }
