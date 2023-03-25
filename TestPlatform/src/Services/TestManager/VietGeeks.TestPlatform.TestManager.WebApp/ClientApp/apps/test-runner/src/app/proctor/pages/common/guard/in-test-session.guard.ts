import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { TestSessionService } from "apps/test-runner/src/app/services/test-session.service";
import { Observable } from "rxjs";

@Injectable()
export class InTestSession implements CanActivate {
  constructor(private _router: Router, private _testSessionsService: TestSessionService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    if (this._testSessionsService.hasSessionData()) {
      return true;
    }

    this._router.navigate(['']);
    return false;
  }
}
