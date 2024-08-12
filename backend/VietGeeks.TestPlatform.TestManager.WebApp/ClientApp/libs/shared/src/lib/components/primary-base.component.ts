import { Component, DestroyRef, inject } from "@angular/core";
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgxSpinnerService } from "ngx-spinner";
import { BehaviorSubject, combineLatest, of, switchMap } from "rxjs";

@Component({ template: '' })
export abstract class PrimaryBaseComponent {
    protected _destroyRef = inject(DestroyRef);

    // There are usually 2 flows, first is for main data, second is for supply data. 
    private _readyForUI = [new BehaviorSubject(false), new BehaviorSubject(false)];

    protected _spinner = inject(NgxSpinnerService);

    get readyForUI$() {
        return combineLatest(this._readyForUI).pipe(switchMap(v => of(!v.includes(false))));
    }

    protected maskBusyForMainFlow() {
        this._readyForUI[0].next(false);
    }

    protected maskReadyForMainFlow() {
        this._readyForUI[0].next(true);
    }

    protected maskBusyForSupplyFlow() {
        this._readyForUI[1].next(false);
    }

    protected maskReadyForSupplyFlow() {
        this._readyForUI[1].next(true);
    }

    protected configureLoadingIndicator() {
        this.readyForUI$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(v => {
            if (!v) {
                this._spinner.show(undefined, {
                    type: 'ball-fussion',
                    size: 'medium',
                    bdColor: 'rgba(100,149,237, .2)',
                    color: 'white',
                    fullScreen: false
                });
            } else {
                this._spinner.hide();
            }
        });
    }
}