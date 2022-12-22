import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, filter } from 'rxjs';

@Component({
  selector: 'viet-geeks-test-specific-layout',
  templateUrl: './test-specific-layout.component.html',
  styleUrls: ['./test-specific-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSpecificLayoutComponent {
  menus$ = new BehaviorSubject<{routerLink: string[], text: string, disable: boolean}[]>([]);

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      const isNewTest = this.route.snapshot.params['testId'] === 'new';
      this.menus$.next([
        {
          routerLink: ['basic-settings'],
          text: 'Basic Settings',
          disable: false
        },
        {
          routerLink: ['manage-questions'],
          text: 'Manage Questions',
          disable: isNewTest
        },
        {
          routerLink: ['test-access'],
          text: 'Test Access',
          disable: isNewTest
        },
        {
          routerLink: ['test-sets'],
          text: 'Test Sets',
          disable: isNewTest
        },
        {
          routerLink: ['test-start-page'],
          text: 'Test Start Page',
          disable: isNewTest
        },
        {
          routerLink: ['time-settings'],
          text: 'Time Settings',
          disable: isNewTest
        },
        {
          routerLink: ['grading-and-summary'],
          text: 'Grading and summary',
          disable: isNewTest
        }
      ]);
    });
  }
}