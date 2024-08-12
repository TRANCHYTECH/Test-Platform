import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { CoreEventsService } from "../services/core-events.service";
import { has } from 'lodash-es';

@Injectable()
export class HttpErrorResponseInterceptor implements HttpInterceptor {
    coreEventsService = inject(CoreEventsService);

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(catchError((errorResponse: HttpErrorResponse) => {
            if (errorResponse.status == 400) {
                let errorMessage = '';
                if (has(errorResponse.error, 'errors')) {
                    errorMessage = errorResponse.error.title;
                } else {
                    errorMessage = errorResponse.error.error;
                }

                this.coreEventsService.httpCallErrors.next(errorMessage);
            }

            return throwError(() => errorResponse);
        }));
    }
}
