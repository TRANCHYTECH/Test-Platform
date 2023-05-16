import { Component, inject, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { PrimaryBaseComponent } from "./primary-base.component";

@Component({ template: '' })
export abstract class EntitySpecificBaseComponent extends PrimaryBaseComponent implements OnInit {
    router = inject(Router);
    protected _refreshAfterSubmit = true;

    abstract loadEntity(): Promise<void>;
    abstract postLoadEntity(): Promise<void> | void;
    abstract submit(): Promise<void>;
    abstract get canSubmit(): boolean;

    ngOnInit(): void {
        // Currently this kind of component only uses one flag for main data flow. So pypass second one.
        this.maskReadyForSupplyFlow();
        this.processLoadingDataFlow();
        this.configureLoadingIndicator();
        this.onInit();
    }

    onInit() {
        // Place holder
    }

    submitFunc = async () => {
        if (!this.canSubmit) {
            return;
        }

        await this.submit();

        // Refresh the page to bind latest info.
        if (this._refreshAfterSubmit) {
            this.processLoadingDataFlow();
        }
    }

    private async processLoadingDataFlow() {
        try {
            this.maskBusyForMainFlow();

            await this.loadEntity();

            await Promise.resolve(this.postLoadEntity());
        } finally {
            this.maskReadyForMainFlow();
        }
    }
}
