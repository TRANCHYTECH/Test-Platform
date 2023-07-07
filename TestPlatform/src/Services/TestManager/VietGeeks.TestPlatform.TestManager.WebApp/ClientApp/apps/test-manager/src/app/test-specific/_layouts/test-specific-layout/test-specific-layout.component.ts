import { AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { getTestId, ToastService, UISupportedService } from '@viet-geeks/shared';
import { BehaviorSubject, filter, Observable } from 'rxjs';
import { TestStatus } from '../../../_state/test-support.model';
import { TestActivationMethodType } from '../../_state/tests/test.model';
import { TestsQuery } from '../../_state/tests/tests.query';
import { TestsService } from '../../_state/tests/tests.service';
import { CoreEventsService } from '@viet-geeks/core';

@Component({
  selector: 'viet-geeks-test-specific-layout',
  templateUrl: './test-specific-layout.component.html',
  styleUrls: ['./test-specific-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSpecificLayoutComponent implements OnInit, AfterViewInit {
  TestStatusRef = TestStatus;
  pageTitle?= 'LOADING TEST';
  sectionTitle$!: Observable<string>;
  breadcrumbs$ = new BehaviorSubject<{ label?: string, active?: boolean }[]>([{ label: 'Loading test' }]);
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);
  testResultMenus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);
  testId!: 'new' | string;
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
  private _destroyRef = inject(DestroyRef);
  private _coreEvents = inject(CoreEventsService);

  constructor() {
    this._router.events.pipe(filter(event => event instanceof NavigationEnd), takeUntilDestroyed(this._destroyRef)).subscribe(() => {
      this.testId = getTestId(this._route);

      const isNewTest = this.testId === 'new';
      if (isNewTest) {
        this.pageTitle = 'New Test';
      }

      this.menus$.next([
        {
          routerLink: ['config', 'basic-settings'],
          text: 'basicSettings',
          icon: 'ri-settings-2-line',
          disable: false
        },
        {
          routerLink: ['config', 'question'],
          text: 'questions',
          icon: 'ri-equalizer-fill',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-sets'],
          text: 'testSets',
          icon: 'ri-tools-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-access'],
          text: 'testAccess',
          icon: 'ri-shield-keyhole-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'test-start-page'],
          text: 'testStartPage',
          icon: 'ri-eye-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'grading-and-summary'],
          text: 'gradingAndSummary',
          icon: 'ri-mark-pen-line',
          disable: isNewTest
        },
        {
          routerLink: ['config', 'time-settings'],
          text: 'timeSettings',
          icon: 'ri-time-line',
          disable: isNewTest
        }
      ]);

      this.testResultMenus$.next([
        {
          routerLink: ['report', 'list'],
          text: 'testResults',
          icon: 'ri-list-check',
          disable: false
        },
        // {
        //   routerLink: ['report', 'statistics'],
        //   text: 'Statistics',
        //   icon: 'ri-time-line',
        //   disable: false
        // },
        // {
        //   routerLink: ['report', 'respondent-monitor'],
        //   text: 'Respondent monitoring',
        //   icon: 'ri-time-line',
        //   disable: false
        // },
        {
          routerLink: ['report', 'test-sheet-review'],
          text: 'testSheetReview',
          icon: 'ri-file-list-3-line',
          disable: false
        }
      ]);
    });
  }

  ngOnInit(): void {
    this._testsQuery.selectActive().pipe(takeUntilDestroyed(this._destroyRef), filter(test => test !== undefined)).subscribe(test => {
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
    this._coreEvents.reasonOfChange.set(`test_${this.testId}_activated`);
    this._notifyService.success('Test activated/scheduled successfully');
  }

  async endTest() {
    await this._testsService.end(this.testId);
    this._coreEvents.reasonOfChange.set(`test_${this.testId}_ended`);
    this._notifyService.success('Test ended successfully');
  }

  async restart() {
    await this._testsService.restart(this.testId);
    this._coreEvents.reasonOfChange.set(`test_${this.testId}_restarted`);
    this._notifyService.success('Test restarted successfully');
    this._router.navigate([this._router.url], { onSameUrlNavigation: 'reload' });
  }
}
