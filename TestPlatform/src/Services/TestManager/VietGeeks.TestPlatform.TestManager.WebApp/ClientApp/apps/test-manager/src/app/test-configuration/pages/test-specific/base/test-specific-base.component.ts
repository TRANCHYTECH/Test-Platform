import { ChangeDetectorRef, Component, inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { ToastService } from "@viet-geeks/shared";
import { NgxSpinnerService } from "ngx-spinner";
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
    changeRef = inject(ChangeDetectorRef);

    testsService = inject(TestsService);
    testsQuery = inject(TestsQuery);

    notifyService = inject(ToastService);
    private _spinner = inject(NgxSpinnerService);

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
                if (testDef === undefined) {
                    await this.router.navigate(['tests']);
                    return;
                }

                this.test = testDef;
            }

            this.afterGetTest();
        });

        this.configureLoadingIndicator();

        this.onInit();
    }

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

    abstract onInit(): void;

    abstract afterGetTest(): void;

    abstract submit(): Promise<void>;

    abstract get canSubmit(): boolean;

    maskReadyForUI() {
        this._readyForUI.next(true);
    }

    submitFunc = async () => {
        if (!this.canSubmit) {
            return;
        }

        await this.submit();
    };

    //todo(tau): how to generalize it?
    setupControlValidityTrigger(parent: FormGroup, sourcePath: string[], targetPaths: string[][]) {
        //todo: improve the destroying subscription.
        parent.get(sourcePath)?.valueChanges.pipe(untilDestroyed(this)).subscribe(() => setTimeout(() => {
            console.log('update trigger');
            targetPaths.forEach(p => {
                const control = parent.get(p);
                control?.updateValueAndValidity();
                control?.markAsTouched();
            });
        }));
    }
}