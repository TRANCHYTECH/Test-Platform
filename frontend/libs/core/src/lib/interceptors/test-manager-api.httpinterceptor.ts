import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CORE_CONFIG } from '../core.config';

export const TestManagerApiHttpInterceptor: HttpInterceptorFn = (req, next) => {
  const coreConfig = inject(CORE_CONFIG);
  const askApiPrefix = '/TestManager:';
  if (req.url.startsWith(askApiPrefix)) {
    const headers = req.headers.set('x-csrf', '1');

    req = req.clone({
      url: req.url.replace(askApiPrefix, coreConfig.testManagerApiBaseUrl),
      headers,
      withCredentials: true,
    });
  }

  return next(req);
};
