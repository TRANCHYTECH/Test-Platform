import { Component } from '@angular/core';
import { projectListWidgets as data } from './data';

@Component({
  selector: 'viet-geeks-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent {
  projectListWidgets: any[] = [];

  constructor() {
    this.projectListWidgets = data;
  }
}
