import { Component, OnInit } from '@angular/core';
import { UiService } from '../state/ui.service';

@Component({
  selector: 'viet-geeks-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
  constructor(private uiService: UiService) {

  }

  ngOnInit(): void {
    this.uiService.add({
      id: 'main',
      routes: [
        { path: '/tests', title: 'Tests', icon: 'ni-tv-2 text-primary', class: '' },
        { path: '/result', title: 'Results', icon: 'ni-pin-3 text-orange', class: '' },
        { path: '/user-profile', title: 'User profile', icon: 'ni-single-02 text-yellow', class: '' },
        { path: '/tables', title: 'Tables', icon: 'ni-bullet-list-67 text-red', class: '' },
        { path: '/login', title: 'Login', icon: 'ni-key-25 text-info', class: '' },
        { path: '/register', title: 'Register', icon: 'ni-circle-08 text-pink', class: '' }
      ]
    });
  }
}
