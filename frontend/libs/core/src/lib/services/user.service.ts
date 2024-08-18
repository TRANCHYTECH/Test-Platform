import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { firstValueFrom, tap, catchError, of } from 'rxjs';
import { CORE_CONFIG } from '../core.config';

export type User = {
  isLoggedIn: boolean;
  name: string;
};

@Injectable({
  providedIn: 'root',
})
export class UserService {
  httpClient = inject(HttpClient);
  router = inject(Router);
  coreConfig = inject(CORE_CONFIG);

  user = computed<User>(() => {
    const user = this._user();

    if (user === null) {
      return {
        isLoggedIn: false,
        name: '',
      };
    }

    return {
      isLoggedIn: true,
      name: user.find((c) => c.type === 'name')?.value as string,
    };
  });

  private _user = signal<{ type: string; value: string | number }[] | null>(
    null
  );

  getUser() {
    return firstValueFrom(
      this.httpClient.get('/TestManager:/bff/user').pipe(
        tap((res: any) => {
          this._user.set(res);
        }),
        catchError((error) => {
          console.log('error get user', error);
          return of(null);
        })
      )
    );
  }

  login() {
    window.location.href = `${this.coreConfig.testManagerApiBaseUrl}/bff/login?returnUrl=/test-portal`;
  }

  logout() {
    const user = this._user();
    if (user === null) return;

    const logoutUrl = user.find((c) => c.type === 'bff:logout_url');
    window.location.href = `${this.coreConfig.testManagerApiBaseUrl}${logoutUrl?.value}&returnUrl=/test-portal`;
  }
}

export const AuthGuard: CanActivateFn = async () => {
  const userService = inject(UserService);
  const coreConfig = inject(CORE_CONFIG);
  const user = await userService.getUser();

  if (user !== null) {
    return true;
  }

  window.location.href = `${coreConfig.testManagerApiBaseUrl}/bff/login?returnUrl=/test-portal`;

  //todo: redirect to unauthorized page. Page has options to login.
  return false;
  //return router.createUrlTree([environment.askApiBaseUrl, "bff", "login"], { queryParams: { returnUrl: '/test-portal' } });
};
