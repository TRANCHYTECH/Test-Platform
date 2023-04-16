import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { getPageTitle } from '@viet-geeks/shared';
import { BehaviorSubject, filter } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-account-layout',
  templateUrl: './account-layout.component.html',
  styleUrls: ['./account-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AccountLayoutComponent {
  pageTitle?= 'User Account';
  sectionTitle?= '';
  menus$ = new BehaviorSubject<{ routerLink: string[], text: string, icon: string, disable: boolean }[]>([]);

  private _router = inject(Router);

  constructor() {
    this._router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
      this.sectionTitle = getPageTitle(this._router);
    });

    this.menus$.next([
      {
        routerLink: ['general-information'],
        text: 'General Information',
        icon: 'ri-settings-2-line',
        disable: false
      },
      {
        routerLink: ['regional-settings'],
        text: 'Language and region',
        icon: 'ri-equalizer-fill',
        disable: false
      }
    ]);
  }
}
