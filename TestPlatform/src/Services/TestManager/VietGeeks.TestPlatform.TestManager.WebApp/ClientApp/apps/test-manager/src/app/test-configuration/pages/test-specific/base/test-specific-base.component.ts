import { ChangeDetectorRef, Component, inject, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { ToastService } from "@viet-geeks/shared";
import { BehaviorSubject, firstValueFrom } from "rxjs";
import { Test } from "../../../state/test.model";
import { TestsQuery } from "../../../state/tests.query";
import { TestsService } from "../../../state/tests.service";

@Component({
    selector: 'viet-geeks-test-specific-base',
    template: ''
})
export abstract class TestSpecificBaseComponent implements OnInit {
    testId!: string;
    test?: Test;

    route = inject(ActivatedRoute);
    changeDetector = inject(ChangeDetectorRef);

    testsService = inject(TestsService);
    testsQuery = inject(TestsQuery);

    notifyService = inject(ToastService);
    
    get readyForUI$() {
        return this._readyForUI.asObservable();
    }

    private _readyForUI = new BehaviorSubject(false);

    ngOnInit(): void {
        this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
            this.testId = p['id'];
            if (this.testId !== 'new') {
                await firstValueFrom(this.testsService.getById(this.testId), { defaultValue: null });
                this.test = this.testsQuery.getEntity(this.testId);
                this.afterGetTest();
            }
        });

        this.onInit();
    }

    abstract onInit(): void;

    abstract afterGetTest(): void;

    maskReadyForUI() {
        this._readyForUI.next(true);
    }
}