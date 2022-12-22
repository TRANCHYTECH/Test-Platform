import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout/layout.component';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from './sidebar/sidebar.component';
import { FooterComponent } from './footer/footer.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TopbarComponent } from './topbar/topbar.component';
import { SharedModule } from '@viet-geeks/shared';

@NgModule({
  declarations: [LayoutComponent, SidebarComponent, FooterComponent, TopbarComponent],
  imports: [CommonModule, RouterModule, NgbModule, SharedModule],
  exports: [LayoutComponent]
})
export class ShellModule {}
