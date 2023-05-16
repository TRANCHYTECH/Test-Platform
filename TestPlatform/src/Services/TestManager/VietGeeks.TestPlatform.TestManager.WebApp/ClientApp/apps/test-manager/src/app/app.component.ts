import { Component, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '@auth0/auth0-angular';
import { TranslateService } from '@ngx-translate/core';
import { CoreEventsService } from '@viet-geeks/core';
import { ToastService } from '@viet-geeks/shared';

@Component({
  selector: 'viet-geeks-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'test-manager';
  notifyService = inject(ToastService);
  authService = inject(AuthService);
  private _destroyRef = inject(DestroyRef);

  constructor(translate: TranslateService, apiErrorNotifyService: CoreEventsService) {
    translate.setDefaultLang('en');
    translate.use('en');

    apiErrorNotifyService.httpCallErrors.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(errorMsg => {
      this.notifyService.error(errorMsg);
    });

    this.authService.isAuthenticated$.subscribe(c => {
      if (c === false) {
        this.authService.loginWithRedirect();
      }
    })
  }
}
