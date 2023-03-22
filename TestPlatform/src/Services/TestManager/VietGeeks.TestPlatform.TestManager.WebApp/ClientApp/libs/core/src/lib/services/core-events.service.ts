import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({providedIn: 'root'})
export class CoreEventsService {
    httpCallErrors = new Subject<string>();
}
