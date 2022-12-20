import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { AuthClientConfig, AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';
import { HttpBackend, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppSettingsService, CoreModule } from '@viet-geeks/core';
import { SharedModule } from '@viet-geeks/shared';
import { AppSettings } from './app-setting.model';
import { environment } from '../environments/environment';
import { of, tap } from 'rxjs';
import { ShellModule } from './shell/shell.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';

@NgModule({
  declarations: [AppComponent],
  imports: [
    CoreModule,
    SharedModule,
    AuthModule.forRoot(),
    BrowserModule,
    ShellModule,
    RouterModule.forRoot(appRoutes, { initialNavigation: 'enabledBlocking' }),
    NgbModule,
    environment.production ? [] : AkitaNgDevtools.forRoot(),
    AkitaNgRouterStoreModule,
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [HttpBackend, AuthClientConfig, AppSettingsService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor, multi: true
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }

function appInitializerFactory(httpBackend: HttpBackend, authClientConfig: AuthClientConfig, appSettingsService: AppSettingsService) {
  return () => {
    if (environment.production) {
      appSettingsService.set<AppSettings>(environment);
      setAuthClientConfig(authClientConfig, environment);

      return of(true);
    }

    return (new HttpClient(httpBackend)).get<AppSettings>('/Configuration')
      .pipe(tap(appSettings => {
        appSettingsService.set(appSettings);
        setAuthClientConfig(authClientConfig, appSettings);
      }));
  }
}

function setAuthClientConfig(authClientConfig: AuthClientConfig, appSettings: AppSettings) {
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

