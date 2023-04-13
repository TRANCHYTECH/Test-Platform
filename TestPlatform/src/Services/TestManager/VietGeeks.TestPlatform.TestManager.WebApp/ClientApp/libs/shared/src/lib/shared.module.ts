import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FeatherModule } from 'angular-feather';
import { allIcons } from 'angular-feather/icons';
import { BreadcrumbsComponent } from './components/breadcrumbs/breadcrumbs.component';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { NgxSpinnerModule } from 'ngx-spinner';
import { SubmitButtonComponent } from './components/submit-button.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { FormsModule } from '@angular/forms';
import { TestStatusClassPipe } from './pipes/test-status-class.pipe';
import { TestStatusPipe } from './pipes/test-status.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    FeatherModule.pick(allIcons),
    NgbToastModule,
    EditorModule,
  ],
  declarations: [BreadcrumbsComponent, SubmitButtonComponent, TestStatusClassPipe, TestStatusPipe],
  exports: [
    CommonModule,
    HttpClientModule,
    TranslateModule,
    FeatherModule,
    BreadcrumbsComponent,
    SubmitButtonComponent,
    SweetAlert2Module,
    NgxSpinnerModule,
    EditorModule,
    TestStatusClassPipe,
    TestStatusPipe
  ]
})
export class SharedModule {}
