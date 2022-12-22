import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApiPrefixInterceptor } from './intercepters/api-prefix.intercepter';
import { AppSettingsService } from './services/app-settings.service';
import { EventService } from './services/event.service';
import { LanguageService } from './services/language.service';

@NgModule({
  imports: [CommonModule],
  providers: [
    AppSettingsService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ApiPrefixInterceptor,
      multi: true
    },
    EventService,
    LanguageService
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded');
    }
  }
}
