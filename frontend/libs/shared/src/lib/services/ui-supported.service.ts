import { DestroyRef, Injectable, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { NavigationEnd, Router } from "@angular/router";
import { BehaviorSubject, filter } from "rxjs";
import { getPageTitle } from "../functions/router-param-functions";

@Injectable({ providedIn: 'root' })
export class UISupportedService {
    private _sectionTitle = new BehaviorSubject<string>('Section title');

    private _router = inject(Router);
    private _destroyRef = inject(DestroyRef);

    get sectionTitle() {
        return this._sectionTitle.asObservable();
    }

    constructor() {
        // Default section title defined in route.
        this._router.events.pipe(filter(event => event instanceof NavigationEnd), takeUntilDestroyed(this._destroyRef)).subscribe(() => {
            this._sectionTitle.next(getPageTitle(this._router));
        });
    }

    // Set custom section title.
    setSectionTitle(value: string) {
        this._sectionTitle.next(value);
    }
}
