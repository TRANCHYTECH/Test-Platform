import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { CoreEventsService } from "../services/core-events.service";

@Injectable()
export class HttpErrorResponseInterceptor implements HttpInterceptor {
    coreEventsService = inject(CoreEventsService);
    
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(catchError((error: HttpErrorResponse) => {
            if (error.status == 400) {
                //todo: process for other bad request errors.
                this.coreEventsService.httpCallErrors.next(error.error['error']);
            }

            return throwError(() => error);
        }));
    }
}
