import { AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { getTestId, ToastService, UISupportedService } from '@viet-geeks/shared';
import { TestStatus } from '@viet-geeks/test-manager/state';
import { BehaviorSubject, filter, Observable } from 'rxjs';
import { TestActivationMethodType } from '../../_state/tests/test.model';
import { TestsQuery } from '../../_state/tests/tests.query';
import { TestsService } from '../../_state/tests/tests.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-specific-layout',
  templateUrl: './test-specific-layout.component.html',
  styleUrls: ['./test-specific-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSpecificLayoutComponent implements OnInit, AfterViewInit {
  pageTitle?= 'LOADING TEST';
  sectionTitle$!: Observable<string>;
  breadcrumbs$ = new BehaviorSubject<{ label?: string, active?: boolean }[]>([{ label: 'Loading test' }]);
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);
  testResultMenus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);
  testId!: string;
  testStatus = TestStatus.Draft;
  activateMethod: 'activate' | 'schedule' | '' = '';
  endMethod: 'endTest' | 'changeSettings' | '' = '';

  private _testsService = inject(TestsService);
  private _testsQuery = inject(TestsQuery);
  private _notifyService = inject(ToastService);
  private _changeRef = inject(ChangeDetectorRef);
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);
  private _uiSupportedService = inject(UISupportedService);

  constructor() {
    this._router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
      this.testId = getTestId(this._route);

      const isNewTest = this.testId === 'new';
      if (isNewTest) {
        this.pageTitle = 'New Test';
      }

      this.menus$.next([
        {
          routerLink: ['config', 'basic-settings'],
          text: 'Basic Settings',
          icon: 'ri-settings-2-line',
          disable: false
        },
        {
          routerLink: ['config', 'question'],
          text: 'Questions',
          icon: 'ri-equalizer-fill',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-sets'],
          text: 'Test Sets',
          icon: 'ri-tools-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-access'],
          text: 'Test Access',
          icon: 'ri-shield-keyhole-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-start-page'],
          text: 'Test Start Page',
          icon: 'ri-eye-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'grading-and-summary'],
          text: 'Grading and summary',
          icon: 'ri-mark-pen-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'time-settings'],
          text: 'Time Settings',
          icon: 'ri-time-line',
          disable: isNewTest
        }
      ]);

      this.testResultMenus$.next([
        {
          routerLink: ['report', 'list'],
          text: 'Test Results',
          icon: 'ri-time-line',
          disable: false
        },
        {
          routerLink: ['report', 'statistics'],
          text: 'Statistics',
          icon: 'ri-time-line',
          disable: false
        },
        {
          routerLink: ['report', 'respondent-monitor'],
          text: 'Respondent monitoring',
          icon: 'ri-time-line',
          disable: false
        }
      ]);
    });
  }

  ngOnInit(): void {
    this._testsQuery.selectActive().pipe(untilDestroyed(this), filter(test => test !== undefined)).subscribe(test => {
      if (test !== undefined) {
        this.pageTitle = test.basicSettings.name;
        this.testStatus = test.status;

        if (test.timeSettings?.testActivationMethod.$type === TestActivationMethodType.ManualTest) {
          this.activateMethod = 'activate';
        } else if (test.timeSettings?.testActivationMethod.$type === TestActivationMethodType.TimePeriod) {
          this.activateMethod = 'schedule';
        }

        if (test.status === TestStatus.Activated) {
          this.endMethod = 'endTest';
        } else if (test.status === TestStatus.Scheduled) {
          this.endMethod = 'changeSettings';
        }

        this._changeRef.markForCheck();
      }
    });

    this.sectionTitle$ =  this._uiSupportedService.sectionTitle;
  }

  ngAfterViewInit(): void {
    document.documentElement.setAttribute('data-sidebar-size', 'sm');
  }

  async activateTest() {
    await this._testsService.activate(this.testId);
    this._notifyService.success('Test activated/scheduled successfully');
  }

  async endTest() {
    await this._testsService.end(this.testId);
    this._notifyService.success('Test ended successfully');
  }

  async restart() {
    await this._testsService.restart(this.testId);
    this._notifyService.success('Test restarted successfully');
    this._router.navigate([this._router.url], { onSameUrlNavigation: 'reload' });
  }
}