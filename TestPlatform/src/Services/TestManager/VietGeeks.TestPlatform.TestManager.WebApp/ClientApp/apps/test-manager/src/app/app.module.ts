import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { AuthClientConfig, AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';
import { HttpBackend, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppSettingsService, CoreModule, HttpErrorResponseInterceptor } from '@viet-geeks/core';
import { SharedModule, EDITOR_API_KEY, ApiErrorHandler } from '@viet-geeks/shared';
import { AppSettings } from './app-setting.model';
import { environment } from '../environments/environment';
import { firstValueFrom, of } from 'rxjs';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';
import { LayoutsModule } from './layouts/layouts.module';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { FlatpickrModule } from 'angularx-flatpickr';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { InputMaskModule } from '@ngneat/input-mask';
import { NgxSpinnerModule } from 'ngx-spinner';
import { provideErrorTailorConfig } from '@ngneat/error-tailor';
import { TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';

const appInitializerFn = (httpBackend: HttpBackend, authClientConfig: AuthClientConfig, appSettingsService: AppSettingsService) => {
  return () => {
    if (!environment.production) {
      appSettingsService.set<AppSettings>(environment);
      setAuthClientConfig(authClientConfig, environment);
      return;
    }

    return firstValueFrom((new HttpClient(httpBackend)).get<AppSettings>('/Configuration')).then(appSettings => {
      appSettingsService.set(appSettings);
      setAuthClientConfig(authClientConfig, appSettings);
    });
  }
}

const setAuthClientConfig = (authClientConfig: AuthClientConfig, appSettings: AppSettings) => {
  authClientConfig.set({
    domain: appSettings.auth.domain,
    clientId: appSettings.auth.clientId,
    audience: appSettings.auth.audience,
    scope: appSettings.auth.scope,
    httpInterceptor: {
      allowedList: appSettings.auth.intercepters
    }
  });
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    CoreModule,
    SharedModule,
    AuthModule.forRoot(),
    BrowserModule,
    BrowserAnimationsModule,
    LayoutsModule,
    RouterModule.forRoot(appRoutes),
    NgbModule,
    environment.production ? [] : AkitaNgDevtools.forRoot(),
    AkitaNgRouterStoreModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    }),
    FlatpickrModule.forRoot(),
    SweetAlert2Module.forRoot(),
    InputMaskModule.forRoot({ inputSelector: 'input', isAsync: true }),
    NgxSpinnerModule.forRoot({ type: 'ball-scale-multiple' })
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      deps: [HttpBackend, AuthClientConfig, AppSettingsService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorResponseInterceptor,
      multi: true
    },
    {
      provide: EDITOR_API_KEY,
      //todo: use configuration from server.
      useValue: 'x0yf00jatpue54s2pib29qju4ql049rbbv602narz7nfx4p2'
    },
    provideErrorTailorConfig({
      errors: {
        useFactory(translateService: TranslateService) {
          return {
            required: () => translateService.instant('errors.required'),
            minlength: error => translateService.instant('errors.minlength', error),
            maxlength: error => translateService.instant('errors.maxlength', error),
            min: error => translateService.instant('errors.min', error),
            max: error => translateService.instant('errors.max', error),
            email: () => translateService.instant('errors.email'),
            url: () => translateService.instant('errors.url'),
            maxNumber: ({ refValues }) => translateService.instant('errors.max', { actual: refValues[0], max: refValues[1] }),
            minNumber: ({ refValues }) => translateService.instant('errors.min', { actual: refValues[0], min: refValues[1] }),
          }
        }, deps: [TranslateService]
      }
    }),
    { provide: TINYMCE_SCRIPT_SRC, useValue: 'assets/tinymce/tinymce.min.js' },
    { 
      provide: ErrorHandler, 
      useClass: ApiErrorHandler 
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}
