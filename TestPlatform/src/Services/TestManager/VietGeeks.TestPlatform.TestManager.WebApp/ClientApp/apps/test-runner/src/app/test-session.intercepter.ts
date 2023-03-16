import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { filter, Observable, tap } from "rxjs";

@Injectable()
export class TestSessionInterceptor implements HttpInterceptor {
    readonly TestSession = 'TestSession';
    private _currentTestSession = '';

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        //todo: remove trick here.
        if (/exam/i.test(req.url)) {
            req = req.clone({
                url: req.url,
                setHeaders: {
                    TestSession: this._currentTestSession
                }
            });

            return next.handle(req).pipe(
                filter(event => event instanceof HttpResponse),
                tap((event) => {
                    const response = event as HttpResponse<never>;
                    if (response.headers.has(this.TestSession)) {
                        this._currentTestSession = response.headers.get(this.TestSession) ?? '';
                    }
                }));
        }

        return next.handle(req);
    }
}