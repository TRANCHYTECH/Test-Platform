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
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      const isNewTest = this.route.snapshot.params['testId'] === 'new';
      this.menus$.next([
        {
          routerLink: ['basic-settings'],
          text: 'Basic Settings',
          icon: 'ri-settings-2-line',
          disable: false
        },
        {
          routerLink: ['manage-questions'],
          text: 'Manage Questions',
          icon: 'ri-equalizer-fill',
          disable: isNewTest
        },
        {
          routerLink: ['test-access'],
          text: 'Test Access',
          icon: 'ri-shield-keyhole-line',
          disable: isNewTest
        },
        {
          routerLink: ['test-sets'],
          text: 'Test Sets',
          icon: 'ri-tools-line',
          disable: isNewTest
        },
        {
          routerLink: ['test-start-page'],
          text: 'Test Start Page',
          icon: 'ri-eye-line',
          disable: isNewTest
        },
        {
          routerLink: ['time-settings'],
          text: 'Time Settings',
          icon: 'ri-time-line',
          disable: isNewTest
        },
        {
          routerLink: ['grading-and-summary'],
          text: 'Grading and summary',
          icon: 'ri-mark-pen-line',
          disable: isNewTest
        }
      ]);
    });
  }
}