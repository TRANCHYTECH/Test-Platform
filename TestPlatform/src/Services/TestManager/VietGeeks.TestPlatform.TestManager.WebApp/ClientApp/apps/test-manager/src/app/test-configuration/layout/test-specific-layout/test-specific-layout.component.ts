import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ToastService } from '@viet-geeks/shared';
import { BehaviorSubject, filter, Subject } from 'rxjs';
import { TestActivationMethodType, TestStatus } from '../../state/test.model';
import { TestsQuery } from '../../state/tests.query';
import { TestsService } from '../../state/tests.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-specific-layout',
  templateUrl: './test-specific-layout.component.html',
  styleUrls: ['./test-specific-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSpecificLayoutComponent implements OnInit {
  title?= '';
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);
  testId!: string;
  testStatus = new Subject<TestStatus>();
  private _testsService = inject(TestsService);
  private _testsQuery = inject(TestsQuery);
  private _notifyService = inject(ToastService);

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
      const testSpecificPartRoute = this.route.snapshot.children[0];
      this.testId = testSpecificPartRoute.params['id'];
      this.title = testSpecificPartRoute.title;
      const isNewTest = this.testId === 'new';
      this.menus$.next([
        {
          routerLink: [this.testId, 'basic-settings'],
          text: 'Basic Settings',
          icon: 'ri-settings-2-line',
          disable: false
        },
        {
          routerLink: [this.testId, 'manage-questions'],
          text: 'Manage Questions',
          icon: 'ri-equalizer-fill',
          disable: isNewTest
        },
        {
          routerLink: [this.testId, 'test-sets'],
          text: 'Test Sets',
          icon: 'ri-tools-line',
          disable: isNewTest
        },
        {
          routerLink: [this.testId, 'test-access'],
          text: 'Test Access',
          icon: 'ri-shield-keyhole-line',
          disable: isNewTest
        },
        {
          routerLink: [this.testId, 'test-start-page'],
          text: 'Test Start Page',
          icon: 'ri-eye-line',
          disable: isNewTest
        },
        {
          routerLink: [this.testId, 'grading-and-summary'],
          text: 'Grading and summary',
          icon: 'ri-mark-pen-line',
          disable: isNewTest
        },
        {
          routerLink: [this.testId, 'time-settings'],
          text: 'Time Settings',
          icon: 'ri-time-line',
          disable: isNewTest
        }
      ]);
    });
  }

  activateMethod: 'activate' | 'schedule' | '' = '';

  ngOnInit(): void {
    this._testsQuery.selectActive().pipe(untilDestroyed(this), filter(test => test !== undefined)).subscribe(test => {
      if (test !== undefined) {
        console.log('test come', test);
        this.testStatus.next(test.status);
        if (test.timeSettings?.testActivationMethod.$type === TestActivationMethodType.ManualTest) {
          this.activateMethod = 'activate';
        } else if (test.timeSettings?.testActivationMethod.$type === TestActivationMethodType.TimePeriod) {
          this.activateMethod = 'schedule';
        }
      }
    });
  }

  async activateTest() {
    await this._testsService.activate(this.testId);
    this._notifyService.success('Test activated successfully');
  }

  async endTest() {
    await this._testsService.end(this.testId);
    this._notifyService.success('Test endded successfully');
  }

}