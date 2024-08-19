import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { LayoutModule } from './layout/layout.module';
import {
  CoreModule,
  provideCore,
  TestManagerApiHttpInterceptor,
} from '@viet-geeks/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { provideErrorTailorConfig } from '@ngneat/error-tailor';
import { SharedModule } from '@viet-geeks/shared';
import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptors,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { environment } from '../environments/environment';
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
    ApiModule.forRoot({ rootUrl: environment.testManagerApiBaseUrl }),
    FingerprintjsProAngularModule.forRoot({
      loadOptions: {
        apiKey: 'JdKT9k9MyKIUAudQFlcB',
        region: 'ap',
      },
    }),
  ],
  providers: [
    provideCore(environment),
    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([TestManagerApiHttpInterceptor])
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TestSessionInterceptor,
      multi: true,
    },
    // TODO: config properly
    provideErrorTailorConfig({
      errors: {
        useValue: {
          required: 'This field is required',
          minlength: ({ requiredLength, actualLength }) =>
            `Expect ${requiredLength} but got ${actualLength}`,
          invalidAddress: (error) => `Address isn't valid`,
        },
      },
    }),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
