import { ChangeDetectorRef, Component, inject, OnInit } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { ToastService } from "@viet-geeks/shared";
import { BehaviorSubject, firstValueFrom } from "rxjs";
import { Test, createTest } from "../../../state/test.model";
import { TestsQuery } from "../../../state/tests.query";
import { TestsService } from "../../../state/tests.service";

@Component({
    selector: 'viet-geeks-test-specific-base',
    template: ''
})
export abstract class TestSpecificBaseComponent implements OnInit {
    testId!: string;
    test!: Test;

    router = inject(Router);
    route = inject(ActivatedRoute);
    changeDetector = inject(ChangeDetectorRef);
    fb = inject(FormBuilder);
    
    testsService = inject(TestsService);
    testsQuery = inject(TestsQuery);

    notifyService = inject(ToastService);
    
    get readyForUI$() {
        return this._readyForUI.asObservable();
    }

    get isNewTest() {
        return this.testId === 'new';
    }

    private _readyForUI = new BehaviorSubject(false);

    constructor() {
        this.test = createTest({});
    }

    ngOnInit(): void {
        this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
            this.testId = p['id'];
            if (!this.isNewTest) {
                await firstValueFrom(this.testsService.getById(this.testId), { defaultValue: null });
                const testDef = this.testsQuery.getEntity(this.testId);
                if(testDef === undefined) {
                    await this.router.navigate(['tests']);
                    return;
                }

                this.test = testDef;
            }

            this.afterGetTest();
        });

        this.onInit();
    }

    abstract onInit(): void;

    abstract afterGetTest(): void;

    maskReadyForUI() {
        this._readyForUI.next(true);
    }
}