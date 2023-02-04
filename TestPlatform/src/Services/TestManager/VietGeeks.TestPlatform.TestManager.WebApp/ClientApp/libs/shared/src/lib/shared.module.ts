import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FeatherModule } from 'angular-feather';
import { allIcons } from 'angular-feather/icons';
import { BreadcrumbsComponent } from './components/breadcrumbs/breadcrumbs.component';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastsContainerComponent } from './components/notifications/toasts-container.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [CommonModule, HttpClientModule, FeatherModule.pick(allIcons), NgbToastModule],
  declarations: [BreadcrumbsComponent, ToastsContainerComponent],
  exports: [
    HttpClientModule,
    TranslateModule,
    FeatherModule,
    BreadcrumbsComponent,
    ToastsContainerComponent
  ]
})
export class SharedModule { }
