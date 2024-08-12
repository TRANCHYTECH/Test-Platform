import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShellComponent } from './shell/shell.component';
import { RouterModule } from '@angular/router';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';

@NgModule({
  declarations: [ShellComponent, FooterComponent, HeaderComponent],
  imports: [CommonModule, RouterModule],
  exports: [ShellComponent],
})
export class LayoutModule {}
