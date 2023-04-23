import { Component, inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { CoreEventsService } from '@viet-geeks/core';
import { ToastService } from '@viet-geeks/shared';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'test-manager';
  notifyService = inject(ToastService);
  authService = inject(AuthService);
  constructor(translate: TranslateService, apiErrorNotifyService: CoreEventsService) {
    translate.setDefaultLang('en');
    translate.use('en');

    apiErrorNotifyService.httpCallErrors.pipe(untilDestroyed(this)).subscribe(errorMsg => {
      this.notifyService.error(errorMsg);
    });

    this.authService.isAuthenticated$.subscribe(c => {
      if (c === false) {
        this.authService.loginWithRedirect();
      }
    })
  }
}
