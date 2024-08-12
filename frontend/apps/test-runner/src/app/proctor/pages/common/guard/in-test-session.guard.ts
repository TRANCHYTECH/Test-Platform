import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from "@angular/router";
import { TestSessionService } from "../../../services/test-session.service";

export const InTestSessionGuard: CanActivateFn =
    (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
      return inject(TestSessionService).checkSessionStatus(state);
    };

