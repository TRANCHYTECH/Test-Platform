import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FeatherModule } from 'angular-feather';
import { allIcons } from 'angular-feather/icons';
import { BreadcrumbsComponent } from './components/breadcrumbs/breadcrumbs.component';

@NgModule({
  imports: [CommonModule, HttpClientModule, FeatherModule.pick(allIcons)],
  declarations: [BreadcrumbsComponent],
  exports: [
    HttpClientModule,
    FeatherModule,
    BreadcrumbsComponent
  ]
})
export class SharedModule {}
