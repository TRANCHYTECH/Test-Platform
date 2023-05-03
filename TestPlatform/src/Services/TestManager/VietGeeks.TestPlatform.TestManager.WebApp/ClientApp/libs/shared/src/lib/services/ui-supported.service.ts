import { Injectable, inject } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { BehaviorSubject, filter } from "rxjs";
import { getPageTitle } from "../functions/router-param-functions";

@UntilDestroy()
@Injectable({ providedIn: 'root' })
export class UISupportedService {
    private _sectionTitle = new BehaviorSubject<string>('Section title');

    private _router = inject(Router);

    get sectionTitle() {
        return this._sectionTitle.asObservable();
    }

    constructor() {
        // Default section title defined in route.
        this._router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
            this._sectionTitle.next(getPageTitle(this._router));
        });
    }

    // Set custom section title.
    setSectionTitle(value: string) {
        this._sectionTitle.next(value);
    }
}
