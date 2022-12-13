import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { AuthClientConfig, AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppSettingsService, CoreModule } from '@viet-geeks/core';
import { SharedModule } from '@viet-geeks/shared';
import { AppSettings } from './app-setting.model';
import { environment } from '../environments/environment';
import { map, of } from 'rxjs';
import { ShellModule } from './shell/shell.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';

@NgModule({
  declarations: [AppComponent],
  imports: [
    CoreModule,
    BrowserModule,
    ShellModule,
    RouterModule.forRoot(appRoutes, { initialNavigation: 'enabledBlocking' }),
    SharedModule,
    AuthModule.forRoot(),
    NgbModule,
    environment.production ? [] : AkitaNgDevtools.forRoot(),
    AkitaNgRouterStoreModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [AppSettingsService, AuthClientConfig],
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

function appInitializerFactory(appSettingsService: AppSettingsService, authClientConfig: AuthClientConfig) {
  return () => {
    if (!environment.production) {
      appSettingsService.set<AppSettings>(environment);
      setAuthClientConfig(authClientConfig, <AppSettings>appSettingsService.appSettings);

      return of(true);
    }

    return appSettingsService.load<AppSettings>().pipe(map(() => {
      setAuthClientConfig(authClientConfig, <AppSettings>appSettingsService.appSettings);

      return of(true);
    }));
  }
}

function setAuthClientConfig(authClientConfig: AuthClientConfig, appSettings: AppSettings) {
  //todo: hardcode here
  authClientConfig.set({
    domain: 'dev-kz3mgkb4xl50tobe.us.auth0.com',
    clientId: 'SaMKFPQSdPoAyooI3zF6oiz6zdZ6hU18',
    audience: 'http://test-manager',
    scope: 'read:current_user',
    httpInterceptor: {
      allowedList: [
        {
          uri: 'http://localhost:8087/*',
          tokenOptions: {
            audience: 'http://test-manager'
          }
        }
      ]
    }
  });
}
