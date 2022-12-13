import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { RouteInfo } from '../state/ui.model';
import { UiQuery } from '../state/ui.query';
import { UiStore } from '../state/ui.store';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {

  public menuItems!: Observable<RouteInfo[]>;
  public isCollapsed = true;

  constructor(private router: Router, private uiQuery: UiQuery) { }

  ngOnInit() {
    this.menuItems = this.uiQuery.selectMainMenus();
    this.router.events.subscribe((event) => {
      this.isCollapsed = true;
   });
  }
}
