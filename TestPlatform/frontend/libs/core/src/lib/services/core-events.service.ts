import { Injectable, computed, signal } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({providedIn: 'root'})
export class CoreEventsService {
    httpCallErrors = new Subject<string>();

    reasonOfChange = signal("");

    testChangedReason = computed(() => {
        const reason = this.reasonOfChange();
        const validReason = (/^test_/i).test(reason);
        
        return validReason ? reason : false;
    });
}