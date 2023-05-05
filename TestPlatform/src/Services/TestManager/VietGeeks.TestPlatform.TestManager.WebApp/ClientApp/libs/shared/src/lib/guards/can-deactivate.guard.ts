import { Observable } from 'rxjs';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

export interface CanComponentDeactivate {
  canDeactivate: () => Observable<boolean>;
}

@Injectable()
export class CanDeactivateGuard  {
  constructor(private readonly router: Router) {}

  canDeactivate(
    component: CanComponentDeactivate,
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    return component.canDeactivate();
  }
}
