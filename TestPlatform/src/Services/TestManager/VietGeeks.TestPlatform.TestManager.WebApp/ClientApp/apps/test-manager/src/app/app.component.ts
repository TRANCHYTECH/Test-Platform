import { Component, inject } from '@angular/core';
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
  constructor(translate: TranslateService, apiErrorNotifyService: CoreEventsService) {
    translate.setDefaultLang('en');
    translate.use('en');

    apiErrorNotifyService.httpCallErrors.pipe(untilDestroyed(this)).subscribe(errorMsg => {
      this.notifyService.error(errorMsg);
    });
  }
}
