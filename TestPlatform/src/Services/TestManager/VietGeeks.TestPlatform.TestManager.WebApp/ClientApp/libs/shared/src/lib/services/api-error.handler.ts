import { ErrorHandler, inject, Injectable } from "@angular/core";
import { ToastService } from "../components/notifications/toast-service";

@Injectable()
export class ApiErrorHandler implements ErrorHandler {

    notifyService = inject(ToastService);

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    handleError(error: any): void {
        console.log('global error', error);
    }
}