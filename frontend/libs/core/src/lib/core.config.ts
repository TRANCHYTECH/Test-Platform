import {
  EnvironmentProviders,
  InjectionToken,
  inject,
  makeEnvironmentProviders,
} from '@angular/core';

export interface CoreConfig {
  testManagerApiBaseUrl: string;
}

export const CORE_CONFIG = new InjectionToken<CoreConfig>('CoreConfig');

export function provideCore<T extends CoreConfig>(
  coreConfig: Partial<T>
): EnvironmentProviders {
  return makeEnvironmentProviders([
    {
      provide: CORE_CONFIG,
      useValue: coreConfig,
    },
  ]);
}

export function injectPortalConfig<T extends CoreConfig>() {
  return inject(CORE_CONFIG) as T;
}
