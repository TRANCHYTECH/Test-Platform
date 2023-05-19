import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { LayoutModule } from './layout/layout.module';
import { AppSettingsService, CoreModule } from '@viet-geeks/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { provideErrorTailorConfig } from '@ngneat/error-tailor';
import { SharedModule } from '@viet-geeks/shared';
import { HttpBackend, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { environment } from '../environments/environment';
import { of, tap } from 'rxjs';
import { AppSettings } from './app-setting.model';
import { TestSessionInterceptor } from './test-session.intercepter';
import { ApiModule } from './api/api.module';
import { FingerprintjsProAngularModule } from '@fingerprintjs/fingerprintjs-pro-angular';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [AppComponent],
  imports: [
    CoreModule,
    SharedModule,
    BrowserModule,
    BrowserAnimationsModule,
    LayoutModule,
    RouterModule.forRoot(appRoutes, { initialNavigation: 'enabledBlocking' }),
    NgbModule,
    ApiModule.forRoot({ rootUrl: environment.testRunnerApiBaseUrl }),
    FingerprintjsProAngularModule.forRoot({loadOptions: {
      apiKey: "JdKT9k9MyKIUAudQFlcB",
      region: "ap"
    }}),
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [HttpBackend, AppSettingsService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TestSessionInterceptor,
      multi: true
    },
    // TODO: config properly
    provideErrorTailorConfig({
      errors: {
        useValue: {
          required: 'This field is required',
          minlength: ({ requiredLength, actualLength }) =>
                      `Expect ${requiredLength} but got ${actualLength}`,
          invalidAddress: error => `Address isn't valid`
        }
      }
    })
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }

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
