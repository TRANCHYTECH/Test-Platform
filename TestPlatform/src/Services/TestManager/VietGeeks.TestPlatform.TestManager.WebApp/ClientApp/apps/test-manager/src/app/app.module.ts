import { HTTP_INTERCEPTORS, HttpBackend, HttpClient } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, NgModule, inject } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { AuthClientConfig, AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { provideErrorTailorConfig } from '@ngneat/error-tailor';
import { InputMaskModule } from '@ngneat/input-mask';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AppSettingsService, CoreModule, HttpErrorResponseInterceptor } from '@viet-geeks/core';
import { ApiErrorHandler, TEXT_EDITOR_CONFIGS, SharedModule, TextEditorConfigs } from '@viet-geeks/shared';
import { FlatpickrModule } from 'angularx-flatpickr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { LayoutsModule } from './_layouts/layouts.module';
import { AppSettings } from './app-setting.model';
import { AppComponent } from './app.component';
import { appRoutes } from './app.routes';

const appInitializerFn = (httpBackend: HttpBackend, authClientConfig: AuthClientConfig, appSettingsService: AppSettingsService) => {
  return () => {
    appSettingsService.set<AppSettings>(environment);
    setAuthClientConfig(authClientConfig, environment);
    // if (!environment.production) {
    //   appSettingsService.set<AppSettings>(environment);
    //   setAuthClientConfig(authClientConfig, environment);
    //   return;
    // }

    // return firstValueFrom((new HttpClient(httpBackend)).get<AppSettings>('/Configuration')).then(appSettings => {
    //   appSettingsService.set(appSettings);
    //   setAuthClientConfig(authClientConfig, appSettings);
    // });
  }
}

const setAuthClientConfig = (authClientConfig: AuthClientConfig, appSettings: AppSettings) => {
  authClientConfig.set({
    domain: appSettings.auth.domain,
    clientId: appSettings.auth.clientId,
    authorizationParams: {
      redirect_uri: window.location.origin,
      audience: appSettings.auth.audience,
      scope: appSettings.auth.scope
    },
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
    RouterModule.forRoot(appRoutes, {
      bindToComponentInputs: true
    }),
    environment.production ? [] : AkitaNgDevtools.forRoot(),
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    }),
    EditorModule,
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
      provide: TEXT_EDITOR_CONFIGS,
      useFactory: textEditorConfigsFn()
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
            inputMask: () => translateService.instant('errors.inputMask'),
            maxHours: (error) => translateService.instant('errors.maxHours', error),
            maxMins: (error) => translateService.instant('errors.maxMins', error),
            maxSeconds: (error) => translateService.instant('errors.maxSeconds', error),
            duration: () => translateService.instant('errors.duration'),
            greaterThanDate: (error) => translateService.instant('errors.greaterThanDate', error),
            furtureDate: () => translateService.instant('errors.furtureDate'),
          }
        }, deps: [TranslateService]
      }
    }),
    {
      provide: ErrorHandler,
      useClass: ApiErrorHandler
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }

function textEditorConfigsFn() {
  return () => {
    const configs = inject(AppSettingsService).get<AppSettings>();
    return <TextEditorConfigs>{ editorApiKey: configs.editorApiKey, uploadPublicKey: configs.uploadPubicKey };
  };
}

function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}
