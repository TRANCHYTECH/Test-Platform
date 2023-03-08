import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { LayoutModule } from './layout/layout.module';
import { AppSettingsService, CoreModule } from '@viet-geeks/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '@viet-geeks/shared';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { of, tap } from 'rxjs';
import { AppSettings } from './app-setting.model';

@NgModule({
  declarations: [AppComponent],
  imports: [
    CoreModule,
    SharedModule,
    BrowserModule,
    LayoutModule,
    RouterModule.forRoot(appRoutes, { initialNavigation: 'enabledBlocking' }),
    NgbModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [HttpBackend, AppSettingsService],
      multi: true
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

function appInitializerFactory(httpBackend: HttpBackend, appSettingsService: AppSettingsService) {
  return () => {
    if (!environment.production) {
      appSettingsService.set<AppSettings>(environment);

      return of(true);
    }

    return (new HttpClient(httpBackend)).get<AppSettings>('/Configuration')
      .pipe(tap(appSettings => {
        appSettingsService.set(appSettings);
      }));
  }
}
