import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { BehaviorSubject, filter } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-specific-layout',
  templateUrl: './test-specific-layout.component.html',
  styleUrls: ['./test-specific-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSpecificLayoutComponent {
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
      const testId = this.route.snapshot.children[0].params['id'];
      const isNewTest = testId === 'new';
      this.menus$.next([
        {
          routerLink: [testId, 'basic-settings'],
          text: 'Basic Settings',
          icon: 'ri-settings-2-line',
          disable: false
        },
        {
          routerLink: [testId, 'manage-questions'],
          text: 'Manage Questions',
          icon: 'ri-equalizer-fill',
          disable: isNewTest
        },
        {
          routerLink: [testId, 'test-access'],
          text: 'Test Access',
          icon: 'ri-shield-keyhole-line',
          disable: isNewTest
        },
        {
          routerLink: [testId, 'test-sets'],
          text: 'Test Sets',
          icon: 'ri-tools-line',
          disable: isNewTest
        },
        {
          routerLink: [testId, 'test-start-page'],
          text: 'Test Start Page',
          icon: 'ri-eye-line',
          disable: isNewTest
        },
        {
          routerLink: [testId, 'time-settings'],
          text: 'Time Settings',
          icon: 'ri-time-line',
          disable: isNewTest
        },
        {
          routerLink: [testId, 'grading-and-summary'],
          text: 'Grading and summary',
          icon: 'ri-mark-pen-line',
          disable: isNewTest
        }
      ]);
    });
  }
}