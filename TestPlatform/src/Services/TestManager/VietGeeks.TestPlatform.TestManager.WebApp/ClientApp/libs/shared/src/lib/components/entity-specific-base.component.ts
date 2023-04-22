import { Component, inject, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { NgxSpinnerService } from "ngx-spinner";
import { BehaviorSubject, filter } from "rxjs";

@UntilDestroy()
@Component({
    selector: 'viet-geeks-entity-specific-base',
    template: ''
})
export abstract class EntitySpecificBaseComponent implements OnInit {
    router = inject(Router);

    protected _spinner = inject(NgxSpinnerService);
    protected _readyForUI = new BehaviorSubject(false);
    protected _refreshAfterSubmit = true;

    get readyForUI$() {
        return this._readyForUI.asObservable();
    }

    abstract loadEntity(): Promise<void>;
    abstract postLoadEntity(): Promise<void> | void;
    abstract submit(): Promise<void>;
    abstract get canSubmit(): boolean;

    ngOnInit(): void {
        this.processLoadingDataFlow();

        // Listen to reload the page.
        this.router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
            console.log('reload page');
            this.processLoadingDataFlow();
        });

        this.configureLoadingIndicator();

        this.onInit();
    }

    onInit() {
        // Place holder
    }

    maskBusyForUI() {
        this._readyForUI.next(false);
    }

    maskReadyForUI() {
        this._readyForUI.next(true);
    }

    submitFunc = async () => {
        if (!this.canSubmit) {
            return;
        }

        await this.submit();

        // Refresh the page to bind latest info.
        if (this._refreshAfterSubmit) {
            this.router.navigate([this.router.url], { onSameUrlNavigation: 'reload' });
        }
    };

    private configureLoadingIndicator() {
        this._readyForUI.pipe(untilDestroyed(this)).subscribe(v => {
            if (v === false) {
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

    private async processLoadingDataFlow() {
        try {
            this.maskBusyForUI();

            await this.loadEntity();

            await Promise.resolve(this.postLoadEntity());
        } finally {
            this.maskReadyForUI();
        }
    }
}
