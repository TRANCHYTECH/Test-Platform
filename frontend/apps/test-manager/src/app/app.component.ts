import { Component, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
  private _destroyRef = inject(DestroyRef);

  constructor(
    translate: TranslateService,
    apiErrorNotifyService: CoreEventsService
  ) {
    translate.setDefaultLang('vi');
    translate.use('vi');

    apiErrorNotifyService.httpCallErrors
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((errorMsg) => {
        this.notifyService.error(errorMsg);
      });
  }
}
